TERMINALBASE= 0xf00
TERMINALDATAREG= 0xf04
TRIGGERTERMINALWRITEINASCII= 0x00050000

Start:
TestSignExtend:
movi r9, -1
addi r7, r0,  Print
movi r1, 'H'
bl r7
movi r1, 'e'
bl r7
movi r1, 'l'
bl r7
movi r1, 'l'
bl r7
movi r1, 'o'
bl r7
movi r1, '!'
bl r7

addi r8, r0, PrintOne
bl r8
HALT

PrintOne:
movi r2, TERMINALBASE
load r3, r2, 0
b lr

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
