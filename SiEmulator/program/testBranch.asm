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
sub  r6, r2, r1
beqz r0, TestBEQZ

HALT
TestBEQZ:
mul r10, r1,r2
bnez r10, testBNEZ
HALT

TestBNEZ:
mul r11, r1, r2
bltz r6, testbltz
HALT

TestBLTZ:
mul r11, r1, r2
bgez r6, TestBGEZ
movi r13, ShouldntBeReached
b r13
HALT

TestBGEZ:
mul r12, r1, r2
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
