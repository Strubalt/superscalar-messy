Start:
TestSignExtend:
movi r9, -1
movi r1, 'H'
addi r7, r0,  Print
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
HALT
#test comment
Print:
movi r2, 10
store r1, r2, 0
b lr
ShouldntBeReached:
movi r0,255
HALT
