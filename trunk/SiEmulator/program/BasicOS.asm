TERMINALBASE= 0xf000
TERMINALDATAREG= 0xf004
TRIGGERTERMINALWRITEINASCII= 0x00050000
TRIGGERTERMINALWRITEININTEGER= 0x00010000
READNEXTDATA=0x00020000
MASK_TIMER_INT=0x1
MASK_TERM_INT=0x2

TCB_LRI_OFFSET=224
TCB_LR_OFFSET=228
TCB_SP_OFFSET=220
TCB_PID_OFFSET=0
TCB_START_ADDRES=4
TCB_REG_OFFSET=16

TIMER_COUNT=100000
SWI_CREATE_PROCESS=0x00
SWI_DELETE_PROCESS=0xFF
SWI_READ_TERMINAL=0x10
SWI_WRITE_TERMINAL=0x11

#--------CODE--------------------------------------------------------------------------

BR Main
BR HWISR
BR SWISR

#Main()-------------------------------------------------------------------------------
Main:
BLR  InitializeOS
XOR  r8, r8, r8

MOVI r0, App1Start
MOVI r1, APP1_TOS
BLR  CreateProcess



BLR  StartOS
HALT

#---------------------------------------------------------------------------------------

#void  InitializeOS()----------------------------------52---------------------------------------------
InitializeOS:

MOVI sp, OS_TOS
SUBI sp, sp, 4
STORE lr,sp, 4

XOR  r8, r8, r8
MOVI r0, IdleProcess
MOVI r1, IDLE_PROCESS_TOS
BLR  CreateProcess
MOVI r9, ODS_IDLE_PID
STORE r1, r9, 0


MOVI r10, OSD_READY_QUEUE
STORE r8, r10, 0

LOAD lr, sp, 4
ADDI sp, sp, 4
RETURN
#---------------------------------------------------------------------------------------



#r1=PID  CreateProcess(r0=processAddr, r1=stack_tos)-----------------108----------------------------
#
#  pid = AllocateTCB(processAddr);
#  while(*queue != 0) { queue++; }
#  *queue = pid;
CreateProcess:
SUBI  sp, sp, 16
STORE lr, sp, 12
STORE r8, sp, 4
STORE r9, sp, 8
STORE r10,sp, 16

BLR AllocateTCB  
MOVI  r8, OSD_READY_QUEUE

CreateProcessLoop:
LOAD  r9, r8, 0
BEQZ  r9, CreateProcessLoopFinish
ADDI  r8, r8, 4
BR    CreateProcessLoop
CreateProcessLoopFinish:
STORE r1, r8, 0

LOAD  r10,sp, 16
LOAD  lr, sp, 12
LOAD  r9, sp, 8
LOAD  r8, sp, 4
ADDI  sp, sp, 16
RETURN


#---------------------------------------------------------------------------------------

#ThreadRoot()---------------------------------------------------------------------------
ThreadRoot:

ENABLE
ADDI  r0, PIDReg, 0
BLR   GetProcessTCB
#r1 now has tcb, load start address into r0
LOAD  r0, r1, TCB_START_ADDRES
BL    r0

#---------------------------------------------------------------------------------------

#r1=PID  AllocateTCB(r0=processAddr, r1=stack_tos)-------------200---------------------------
AllocateTCB:

SUBI  sp, sp, 16
STORE r2, sp, 4
STORE r3, sp, 8
STORE r4, sp, 12
STORE r5, sp, 16

NOP
ADDI  r5, r1, 0
MOVI  r2, OSD_NUM_PROCESS_CREATED
LOAD  r1, r2, 0
ADDI  r1, r1, 1
STORE r1, r2, 0
MOVI  r4, ThreadRoot
AllocateTCB_1:
MOVI  r2, OSD_TCB1
LOAD  r3, r2, TCB_PID_OFFSET
BNEZ  r3, AllocateTCB_2
STORE r1, r2, TCB_PID_OFFSET
STORE r0, r2, TCB_START_ADDRES
ADDI  r3, r2, TCB_SP_OFFSET
STORE r5, r3, 0
STORE r4, r3, 8
BR    AllocateTCBFinish

AllocateTCB_2:
MOVI  r2, OSD_TCB2
LOAD  r3, r2, TCB_PID_OFFSET
BNEZ  r3, AllocateTCB_3
STORE r1, r2, TCB_PID_OFFSET
STORE r0, r2, TCB_START_ADDRES
ADDI  r3, r2, TCB_SP_OFFSET
STORE r5, r3, 0
STORE r4, r3, 8

BR    AllocateTCBFinish

AllocateTCB_3:
MOVI  r2, OSD_TCB3
LOAD  r3, r2, TCB_PID_OFFSET
BNEZ  r3, AllocateTCB_4
STORE r1, r2, TCB_PID_OFFSET
STORE r0, r2, TCB_START_ADDRES
ADDI  r3, r2, TCB_SP_OFFSET
STORE r5, r3, 0
STORE r4, r3, 8
BR    AllocateTCBFinish

AllocateTCB_4:
MOVI  r2, OSD_TCB4
LOAD  r3, r2, TCB_PID_OFFSET
BNEZ  r3, Error
STORE r1, r2, TCB_PID_OFFSET
STORE r0, r2, TCB_START_ADDRES
ADDI  r3, r2, TCB_SP_OFFSET
STORE r5, r3, 0
STORE r4, r3, 8
BR    AllocateTCBFinish

AllocateTCBFinish:

LOAD  r2, sp, 4
LOAD  r3, sp, 8
LOAD  r4, sp, 12
LOAD  r5, sp, 16
ADDI  sp, sp, 16
RETURN

#---------------------------------------------------------------------------------------


#StartOS()------------------------------------372-------------------------------------------
StartOS:


MOVI r0, 1
MOVI r1, OSD_HAS_STARTED
STORE r0, r1, 0

movi timerReg, TIMER_COUNT

#Branch To RunNewThread, It will pickup a new process and do context switch.
#After context switch, LR will become ThreadRoot Address
#So when RETURN, it will jump to ThreadRoot and execute new process

BLR  RunNewThread

HALT

Loop1:
movi r0, 0

bnez r16, Loop1
HALT


#------------------------------------------------------------------------------------

#r1=PID PickNewProcess()------------------------------408---------------------------------
#		N.B.:Idle process is not in the queue
#   PID=*queue;		//PID: r1, queue r8
#   while(*(queue+1) != 0) {
#      *queue=*(queue+1);
#      queue+=1;
#   } 
#   *queue=PID;
#   if(PID==0) PID = IDLE_PID;

PickNewProcess:
subi  sp, sp, 8
store r8, sp, 4
store r9, sp, 8

MOVI  r8, OSD_READY_QUEUE
LOAD  r1, r8, 0

PickNewProcessLoop:
LOAD  r9, r8, 4
BEQZ  r9, PickNewProcessLoopFinish
STORE r9,r8, 0
ADDI  r8, r8, 4
BR    PickNewProcessLoop

PickNewProcessLoopFinish:
STORE r1,r8, 0
BNEZ  r1, PickNewProcessFinish
MOVI  r9, ODS_IDLE_PID
LOAD  r1, r9, 0

PickNewProcessFinish:
load  r8, sp, 4
load  r9, sp, 8
addi  sp, sp, 8
b     lr
#------------------------------------------------------------------------------------


#RunNewThread()--------------------------------480-----------------------------------------------
RunNewThread:


SUBI  sp, sp, 16
STORE LR,sp, 4
STORE r0,sp, 8
STORE r1,sp, 12
STORE r2,sp, 16

BLR   PickNewProcess
SUB   r0, PIDReg, r1
BEQZ  r0, CS_THREAD_NOT_CHANGE
BNEZ  PIDReg, CS_STORE_CUR_CONTEXT

#Originally no process is running. Restore sp, switch to new process
ADDI  sp, sp, 16
ADDI  PIDReg, r1, 0
BR    CS_SWITCH_TO_NEW

CS_STORE_CUR_CONTEXT:
  # need to restore sp ...

ADDI  R0, PIDReg, 0
ADDI  PIDReg, r1, 0
BLR   GetProcessTCB
ADDI  R1, R1, TCB_REG_OFFSET

# register r0~r2 is incorrect
STORE R3, R1, 12
STORE R4, R1, 16
STORE R5, R1, 20
STORE R6, R1, 24
STORE R7, R1, 28
STORE R8, R1, 32
STORE R9, R1, 36
STORE R10, R1, 40
STORE R11, R1, 44
STORE R12, R1, 48
STORE R13, R1, 52
STORE R14, R1, 56
STORE R15, R1, 60
STORE R16, R1, 64
STORE R17, R1, 68
STORE R18, R1, 72
STORE R19, R1, 76
STORE R20, R1, 80
STORE R21, R1, 84
STORE R22, R1, 88
STORE R23, R1, 92
STORE R24, R1, 96
STORE R25, R1, 100
STORE R26, R1, 104
STORE R27, R1, 108
STORE R28, R1, 112
STORE R29, R1, 116

STORE R30, R1, 120
STORE R31, R1, 124

#Need to add 128 because store can only accept immediate between -128~127
ADDI  R1,  R1, 128
STORE R32, R1, 0
STORE R33, R1, 4
STORE R34, R1, 8
STORE R35, R1, 12
STORE R36, R1, 16
STORE R37, R1, 20
STORE R38, R1, 24
STORE R39, R1, 28
STORE R40, R1, 32
STORE R41, R1, 36
STORE R42, R1, 40
STORE R43, R1, 44
STORE R44, R1, 48
STORE R45, R1, 52
STORE R46, R1, 56
STORE R47, R1, 60
STORE R48, R1, 64
STORE R49, R1, 68
STORE R50, R1, 72
# stack pointer is incorrect now STORE R51, R1, 76
STORE R52, R1, 80
# lr is incorrect,               STORE R53, R1, 84

# tcb->regs+128 ==> R3

ADDI  R3,  R1, 0

LOAD  LR,sp, 4
LOAD  r0,sp, 8
LOAD  r1,sp, 12
LOAD  r2,sp, 16
ADDI  sp, sp, 16

STORE SP, R3, 76
STORE LR, R3, 84

SUBI  R3, R3, 128

STORE R0, R3, 0
STORE R1, R3, 4
STORE R2, R3, 8


BR    CS_SWITCH_TO_NEW

CS_THREAD_NOT_CHANGE:

LOAD  LR,sp, 4
LOAD  r0,sp, 8
LOAD  r1,sp, 12
LOAD  r2,sp, 16
ADDI  sp, sp, 16
BR    CS_FINISH

CS_SWITCH_TO_NEW:

# get current process tcb address-> r1
ADDI  r0, PIDReg, 0

SUBI  sp, sp, 4
STORE lr, sp, 4
BLR   GetProcessTCB
LOAD  lr, sp, 4
ADDI  sp, sp, 4

# set TCB address to r0
ADDI  r0, r1, 0
STORE PIDReg, r0, TCB_PID_OFFSET
ADDI  r0, r0, TCB_REG_OFFSET           
LOAD  r1, r0, 4
LOAD  r2, r0, 8
LOAD  r3, r0, 12
LOAD  r4, r0, 16
LOAD  r5, r0, 20
LOAD  r6, r0, 24
LOAD  r7, r0, 28
LOAD  r8, r0, 32
LOAD  r9, r0, 36
LOAD  r10, r0, 40
LOAD  r11, r0, 44
LOAD  r12, r0, 48
LOAD  r13, r0, 52
LOAD  r14, r0, 56
LOAD  r15, r0, 60
LOAD  r16, r0, 64
LOAD  r17, r0, 68
LOAD  r18, r0, 72
LOAD  r19, r0, 76
LOAD  r20, r0, 80
LOAD  r21, r0, 84
LOAD  r22, r0, 88
LOAD  r23, r0, 92
LOAD  r24, r0, 96
LOAD  r25, r0, 100
LOAD  r26, r0, 104
LOAD  r27, r0, 108
LOAD  r28, r0, 112
LOAD  r29, r0, 116
LOAD  r30, r0, 120
LOAD  r31, r0, 124

ADDI  r0,  r0, 128
LOAD  r32, r0, 0
LOAD  r33, r0, 4
LOAD  r34, r0, 8
LOAD  r35, r0, 12
LOAD  r36, r0, 16
LOAD  r37, r0, 20
LOAD  r38, r0, 24
LOAD  r39, r0, 28
LOAD  r40, r0, 32
LOAD  r41, r0, 36
LOAD  r42, r0, 40
LOAD  r43, r0, 44
LOAD  r44, r0, 48
LOAD  r45, r0, 52
LOAD  r46, r0, 56
LOAD  r47, r0, 60
LOAD  r48, r0, 64
LOAD  r49, r0, 68
LOAD  r50, r0, 72
LOAD  r51, r0, 76
LOAD  r52, r0, 80
LOAD  r53, r0, 84
LOAD  r0, r0, -128
BR    CS_FINISH

CS_FINISH:
RETURN
#------------------------------------------------------------------------------------

#r1=TCBAddr GetProcessTCB(r0=pid)-------------------------------------------------------------------------------
GetProcessTCB:
SUBI sp, sp, 8
STORE r8, sp, 4
STORE r9, sp, 8

MOVI r1, OSD_TCB1
LOAD r8, r1, TCB_PID_OFFSET
SUB  r9, r8, r0
BEQZ r9, GetProcessTCB_FINISH
MOVI r1, OSD_TCB2
LOAD r8, r1, TCB_PID_OFFSET
SUB  r9, r8, r0
BEQZ r9, GetProcessTCB_FINISH
MOVI r1, OSD_TCB3
LOAD r8, r1, TCB_PID_OFFSET
SUB  r9, r8, r0
BEQZ r9, GetProcessTCB_FINISH
MOVI r1, OSD_TCB4
LOAD r8, r1, TCB_PID_OFFSET
SUB  r9, r8, r0
BNEZ r9, Error

GetProcessTCB_FINISH:
LOAD r8, sp, 4
LOAD r9, sp, 8
ADDI sp, sp, 8
b lr
#------------------------------------------------------------------------------------



#Error()-------------------------------------------------------------------------------
Error:
HALT

#------------------------------------------------------------------------------------

# Printk(r0=data)		11xx
Printk:
subi  sp, sp, 8
store r8, sp, 4
store r9, sp, 8

movi  r8, TERMINALDATAREG
store r0, r8, 0
movi  r8, TERMINALBASE
movi  r9, TRIGGERTERMINALWRITEININTEGER
store r9, r8, 0

load  r8, sp, 4
load  r9, sp, 8
addi  sp, sp, 8
b lr


# SWISR(r0=code, r1, r2) ---------------------------------------------------------------------
SWISR:	
SUBI  R0, R0, SWI_CREATE_PROCESS
BEQZ  R0, SWICreateProcess
ADDI  R0, R0, SWI_CREATE_PROCESS
SUBI  R0, R0, SWI_WRITE_TERMINAL
BEQZ  R0, SWIWriteToTerminal
RESUME


# SWICreateProcess(r1=processAddr, r2=stack_tos)--------------------------------
SWICreateProcess:
ADDI r0, r1, 0
ADDI r1, r2, 0
BLR  CreateProcess
MOVI r0, 0
RESUME

# WriteToTerminal(r1=terminal#, r2=data)----------------------------------
SWIWriteToTerminal:
ADDI  r0, r2, 0
BLR   Printk
MOVI  r0, 0
RESUME

HWISR:
subi  sp, sp, 8
store r8, sp, 4
store r9, sp, 8

addi r8, interruptReg, 0
andi r9, r8, MASK_TIMER_INT
bnez r9, TIMERISR
br   TERMINALISR

TIMERISR:
#clear timer 
movi r8, 0
nori r9, r8, MASK_TIMER_INT
and  r58, r58, r9

load  r8, sp, 4
load  r9, sp, 8
addi  sp, sp, 8

movi timerReg, TIMER_COUNT
BLR  RunNewThread
RESUME


TERMINALISR:

subi sp, sp, 4
store r10, sp, 4
movi r8, 0
nori r9, r8, MASK_TERM_INT
and  r58, r58, r9

#clear data
movi r8, READNEXTDATA
movi r9, TERMINALBASE
load r10, r9, 8		
store r8, r9, 0

addi r7, r0,  Printk
add r1, r0, r10
bl r7

load  r10,sp, 4
load  r8, sp, 8
load  r9, sp, 12
addi  sp, sp, 12
RESUME



OS_GLOBAL_DATA:

OSD_HAS_STARTED:
DATA 0
OSD_NUM_PROCESS_CREATED:
DATA 0
OSD_SP:
DATA 0
ODS_IDLE_PID:
DATA 0

OSD_READY_QUEUE:
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0

OSD_TERM_READ_QUEUE:
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0


OSD_TCB1:
#status: PID
DATA 0
DATA 0
DATA 0
DATA 0
#registers: r0~r53
#0~9
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#10~19
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#20~29
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#30~39
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#40~49
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#50~53
DATA 0
DATA 0
DATA 0
DATA 0


OSD_TCB2:
#status: PID
DATA 0
DATA 0
DATA 0
DATA 0
#registers: r0~r53
#0~9
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#10~19
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#20~29
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#30~39
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#40~49
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#50~53
DATA 0
DATA 0
DATA 0
DATA 0

OSD_TCB3:
#status: PID
DATA 0
DATA 0
DATA 0
DATA 0
#registers: r0~r53
#0~9
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#10~19
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#20~29
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#30~39
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#40~49
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#50~53
DATA 0
DATA 0
DATA 0
DATA 0


OSD_TCB4:
#status: PID
DATA 0
DATA 0
DATA 0
DATA 0
#registers: r0~r53
#0~9
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#10~19
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#20~29
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#30~39
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#40~49
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#50~53
DATA 0
DATA 0
DATA 0
DATA 0


OS_STACK:
#0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#1
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#2
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#3
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
OS_TOS:
DATA 0


IDLE_PROCESS_STACK:
#0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#1
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#2
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#3
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
IDLE_PROCESS_TOS:
DATA 0

#---------------------------------------------------------------------------------
#---------------------------------------------------------------------------------
#--Process Codes-----------------------------------------------------------------
#---------------------------------------------------------------------------------
#---------------------------------------------------------------------------------

#
IdleProcess:
IdleProcessStart:
ADDI r8, r8, 1
BR   IdleProcessStart


#App1
App1Start:

MOVI r1, App2Start
MOVI r2, APP2_TOS
MOVI r0, SWI_CREATE_PROCESS
SWI  
MOVI r1, App3Start
MOVI r2, APP3_TOS
MOVI r0, SWI_CREATE_PROCESS
SWI  

MOVI r1, 1
MOVI r8, 1
MOVI r0, SWI_WRITE_TERMINAL
MOVI r9, 0
MOVI r11,1

App1LoopStart:
ADDI r2, r8, 0
MOVI r10, 1000
SUB  r10, r9, r10
BNEZ r10, SKIP_SWI1
MOVI r0, SWI_WRITE_TERMINAL
SWI  
ADDI r2, r11, 0
MOVI r0, SWI_WRITE_TERMINAL
SWI  
ADDI r8, r8, 3
MOVI r9, 0

SKIP_SWI1:
ADDI r9, r9, 1
BR   App1LoopStart
B    LR

#App2
App2Start:
MOVI r1, 1
MOVI r8, 0
MOVI r9, 0
MOVI r11,2

App2LoopStart:
ADDI r2, r8, 0
MOVI r10, 1000
SUB  r10, r9, r10
BNEZ r10, SKIP_SWI2
MOVI r0, SWI_WRITE_TERMINAL
SWI  
ADDI r2, r11, 0
MOVI r0, SWI_WRITE_TERMINAL
SWI  
ADDI r8, r8, 3
MOVI r9, 0
SKIP_SWI2:
ADDI r9, r9, 1
BR   App2LoopStart
B    LR


#App3
App3Start:
MOVI r1, 1
MOVI r8, 2
MOVI r9, 0
MOVI r11,3

App3LoopStart:
ADDI r2, r8, 0
MOVI r10, 1000
SUB  r10, r9, r10
BNEZ r10, SKIP_SWI3
MOVI r0, SWI_WRITE_TERMINAL
SWI  
ADDI r2, r11, 0
MOVI r0, SWI_WRITE_TERMINAL
SWI  
ADDI r8, r8, 3
MOVI r9, 0
SKIP_SWI3:
ADDI r9, r9, 1
BR   App3LoopStart
B    LR



#---------------------------------------------------------------------------------
#---------------------------------------------------------------------------------
#--Process Stacks-----------------------------------------------------------------
#---------------------------------------------------------------------------------
#---------------------------------------------------------------------------------





APP1_STACK:
#0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#1
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#2
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#3
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
APP1_TOS:
DATA 0


APP2_STACK:
#0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#1
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#2
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#3
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
APP2_TOS:
DATA 0



APP3_STACK:
#0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#1
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#2
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#3
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
APP3_TOS:
DATA 0





APP4_STACK:
#0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#1
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#2
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
#3
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
DATA 0
APP4_TOS:
DATA 0


