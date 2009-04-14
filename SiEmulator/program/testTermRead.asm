TERMINALBASE= 0xf00
TERMINALDATAREG= 0xf04
TRIGGERTERMINALWRITEINASCII= 0x00050000
READNEXTDATA=0x00020000
a7= 0x80
a8= 0x100
First:
BR Start
BR HWISR
BR SWISR

Start:
movi r58, a7
muli  r58, r58, a8
muli  r58, r58, a8
muli  r58, r58, a8
movi r13, 200
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
subi r13, r13, 1
addi r14, r14, 2
bnez r13, TestSignExtend

HALT

HWISR:
addi r30, r30, 1
movi r31, READNEXTDATA
movi r32, TERMINALBASE
load r34, r32, 8
store r31, r32, 0
movi r35, 1001
addi r7, r0,  Print
movi r1, 'r'
bl r7
movi r1, 'e'
bl r7
movi r1, 'c'
bl r7
movi r1, 'e'
bl r7
movi r1, 'i'
bl r7
movi r1, 'v'
bl r7
movi r1, 'e'
bl r7
movi r1, ':'
bl r7
add r1, r0, r34
bl r7
movi r1, '\n'
bl r7
RESUME

SWISR:
movi r31, 1000

RESUME

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
