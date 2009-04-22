TERMINALBASE= 0xf000
TERMINALDATAREG= 0xf004
TRIGGERTERMINALWRITEINASCII= 0x00050000

Start:
TestSignExtend:
movi r0, 0
movi r1, 4
movi r2, 2
add  r3, r1, r2
sub  r4, r1, r2
mul  r5, r1, r2
div  r6, r1, r2
and  r7, r1, r2
or   r8, r1, r2
xor  r9, r1, r2
nand r10, r1, r2
nor  r11, r1, r2
nxor r12, r1, r2

addi r14, r1, -1
subi r15, r1, -1
muli r16, r1, -1
divi r17, r1, -1
andi r18, r1, -1
ori  r19, r1, -1
xori r20, r1, -1
nandi r21, r1, -1
nori r22, r1, -1
nxori r23, r1, -1

movi r24, 4
movi r25, -10
store r25, r24, 0
load  r26, r24, 0

movi r27, 100
swap r27, r24, 0
load r28, r24, 0

HALT

Print:
movi r2, TERMINALDATAREG
store r1, r2, 0
movi r2, TERMINALBASE
movi r1, TRIGGERTERMINALWRITEINASCII
or r1, r1, r10
store r1, r2, 0
b lr
ShouldntBeReached:
movi r0,255
HALT
