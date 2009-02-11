Start:
AAAA= 0x10
TestSignExtend:
movi r9, -1
movi r1, 'H'
addi r7, r0,  Print
bl r7
movi r1, 'a'
bl r7
movi r1, 'b'
bl r7
movi r1, 'c'
bl r7
movi r1, 'd'
bl r7
movi r1, 'e'
bl r7
movi r1, 'A'
bl r7
movi r1, 'B'
bl r7
movi r1, 'C'
bl r7
movi r1, 'D'
bl r7
movi r1, '\t'
bl r7
HALT
Print:
movi r2, 10
store r1, r2, 0
b lr
ShouldntBeReached:
movi r0,255
HALT
