NOTOPRINT= 5
TERMINALDATAADDRESS= 10
Start:
# move first Fibonacci number into r1
movi r1, 1
# move (n-1)th Fibonacci number into r2
movi r2, 1
# initialise loop counter
movi r8, NOTOPRINT
blr Print
subi r8, r8, 0x01
Loop:
# calculate (n+1)th number
add r3, r1, r2
# shift everything along a bit
#mov r2, r1 is sythesised as:
addi r2, r1, 0
# mov r1, r3
addi r1, r3, 0
blr Print
subi r8, r8, 0x01
bnez r8, Loop
HALT
#
Print:
movi r10, TERMINALDATAADDRESS
store r1, r10, 0
return
HALT
