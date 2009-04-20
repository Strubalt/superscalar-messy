TERMINALBASE= 0xf00
TERMINALDATAREG= 0xf04
TRIGGERTERMINALWRITEINASCII= 0x00050000
TRIGGERTERMINALWRITEININTEGER= 0x00010000
READNEXTDATA=0x00020000
MASK_TIMER_INT=0x1
MASK_TERM_INT=0x2

TCB_LRI_OFFSET=224
TCB_LR_OFFSET=228
TCB_PID_OFFSET=0
TCB_REG_OFFSET=16
TIMER_COUNT=1000

BR Main
BR HWISR
BR SWISR

#Main()-------------------------------------------------------------------------------
Main:
BLR  InitializeOS
XOR  r8, r8, r8

ADDI r0, r8, App1Start
BLR  CreateProcess

ADDI r0, r8, App2Start
BLR  CreateProcess

BLR  StartOS
HALT

#---------------------------------------------------------------------------------------

#void  InitializeOS()-------------------------------------------------------------------------------
InitializeOS:
XOR  sp, sp, sp
ADDI sp, sp, OS_TOS

XOR  r8, r8, r8
ADDI r0, r8, IdleProcess
BLR  CreateProcess
MOVI r9, ODS_IDLE_PID
STORE r1, r9, 0

MOVI r10, OSD_READY_QUEUE
STORE r8, r10, 0
#---------------------------------------------------------------------------------------



#r1=PID  CreateProcess(r0=processAddr)---------------------------------------------------------
#
#  pid = AllocateTCB(processAddr);
#  while(*queue != 0) { queue++; }
#  *queue = pid;
CreateProcess:
SUBI  sp, sp, 12
STORE lr, sp, 12
STORE r9, sp, 8
STORE r8, sp, 4

BLR AllocateTCB  
MOVI  r8, OSD_READY_QUEUE

CreateProcessLoop:
LOAD  r9, r8, 0
BEQZ  r9, CreateProcessLoopFinish
ADDI  r8, r8, 4
BR    CreateProcessLoop
CreateProcessLoopFinish:

LOAD  lr, sp, 12
LOAD  r9, sp, 8
LOAD  r8, sp, 4
ADDI  sp, sp, 12
RETURN


#---------------------------------------------------------------------------------------

#ThreadRoot()---------------------------------------------------------------------------
ThreadRoot:

RESUME
#---------------------------------------------------------------------------------------

#r1=PID  AllocateTCB(r0=processAddr)-------------------------------------------------------------------------------
AllocateTCB:
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
STORE r0, r2, TCB_LRI_OFFSET
STORE r4, r2, TCB_LR_OFFSET
BR    AllocateTCBFinish

AllocateTCB_2:
MOVI  r2, OSD_TCB2
LOAD  r3, r2, TCB_PID_OFFSET
BNEZ  r3, AllocateTCB_3
STORE r1, r2, TCB_PID_OFFSET
STORE r0, r2, TCB_LRI_OFFSET
STORE r4, r2, TCB_LR_OFFSET
BR    AllocateTCBFinish

AllocateTCB_3:
MOVI  r2, OSD_TCB3
LOAD  r3, r2, TCB_PID_OFFSET
BNEZ  r3, AllocateTCB_4
STORE r1, r2, TCB_PID_OFFSET
STORE r0, r2, TCB_LRI_OFFSET
STORE r4, r2, TCB_LR_OFFSET
BR    AllocateTCBFinish

AllocateTCB_4:
MOVI  r2, OSD_TCB4
LOAD  r3, r2, TCB_PID_OFFSET
BNEZ  r3, Error
STORE r1, r2, TCB_PID_OFFSET
STORE r0, r2, TCB_LRI_OFFSET
STORE r4, r2, TCB_LR_OFFSET
BR    AllocateTCBFinish

AllocateTCBFinish:
RETURN

#---------------------------------------------------------------------------------------


#StartOS()-------------------------------------------------------------------------------
StartOS:


MOVI r0, 1
MOVI r1, OSD_HAS_STARTED
STORE r0, r1, 0

movi timerReg, TIMER_COUNT

#Set r1 from PickNewProcess
BLR PickNewProcess
MOVI r0, 0
BLR ContextSwitch
RESUME
HALT

Loop1:
movi r0, 0

bnez r16, Loop1
HALT


#------------------------------------------------------------------------------------

#r1=PID PickNewProcess()---------------------------------------------------------------
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

#ContextSwitch(r0=curentPID r1=newPID)-------------------------------------------------------------------------------
ContextSwitch:
SUBI sp, sp, 
SUBI r8, r0, r1
BEQZ r8, CS_FINISH
ADDI r8, r0, 0
ADDI r9, r1, 0

BEQZ r0, CS_SWITCH_TO_NEW
#get current process tcb address-> r1
BLR   GetProcessTCB
ADDI  r1, r1, TCB_REG_OFFSET           
STORE r1, 

CS_STORE_CURRENT_CONTEXT:


CS_SWITCH_TO_NEW:

CS_FINISH:
b lr
#------------------------------------------------------------------------------------

#r1=TCBAddr GetProcessTCB(r0=pid)-------------------------------------------------------------------------------
GetProcessTCB:
SUBI sp, sp, 8
STORE r8, sp, 4
STORE r9, sp, 8

MOVI r1, OSD_TCB1
LOAD r8, r1, TCB_PID_OFFSET
SUBI r9, r8, r0
BEQZ r9, GetProcessTCB_FINISH
MOVI r1, OSD_TCB2
LOAD r8, r1, TCB_PID_OFFSET
SUBI r9, r8, r0
BEQZ r9, GetProcessTCB_FINISH
MOVI r1, OSD_TCB3
LOAD r8, r1, TCB_PID_OFFSET
SUBI r9, r8, r0
BEQZ r9, GetProcessTCB_FINISH
MOVI r1, OSD_TCB4
LOAD r8, r1, TCB_PID_OFFSET
SUBI r9, r8, r0
BEQZ r9, Error

GetProcessTCB_FINISH:
LOAD r8, sp, 4
LOAD r9, sp, 8
b lr
#------------------------------------------------------------------------------------



#Error()-------------------------------------------------------------------------------
Error:
HALT

#------------------------------------------------------------------------------------


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




SWISR:
movi r31, 1000
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
BLR  PickNewProcess
ADDI r0, PIDReg, 0
BLR  ContextSwitch
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
DATA 0
#4
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
#5
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
#6
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
#7
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
#8
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
#9
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
#10
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
#11
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
#12
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
#13
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
#14
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
#15
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
#16
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
#17
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
#18
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
#19
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
#20
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

IdleProcess:
IdleProcessStart:
ADDI r8, r8, 1
BR   IdleProcessStart

#App1
App1Start:
MOVI r1, 1
MOVI r8, 1
MOVI r0, 0x11

App1LoopStart:
ADDI r2, r8, 0
SWI  
ADDI r8, r8, 2
BR   App1LoopStart

#App2
App2Start:
MOVI r1, 1
MOVI r8, 0
MOVI r0, 0x11

App2LoopStart:
ADDI r2, r8, 0
SWI  
ADDI r8, r8, 2
BR   App2LoopStart








