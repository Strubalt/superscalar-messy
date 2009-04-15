TERMINALBASE= 0xf00
TERMINALDATAREG= 0xf04
TRIGGERTERMINALWRITEINASCII= 0x00050000
READNEXTDATA=0x00020000

MASK_TIMER_INT=0x1
MASK_TERM_INT=0x2

BR Main
BR HWISR
BR SWISR

Main:
Search10Prime:
	movi r8, 9
	movi r9, 3
 Search10PrimeLoop:
	beqz r8, Finish
	movi r0, 0
	add  r0, r0, r9
	blr  IsPrime
	addi r9, r9, 1
	beqz r1, SearchLoopBack
	subi r8, r8, 1
 SearchLoopBack:
	br   Search10PrimeLoop
 Finish:
	HALT

IsPrime:
	movi r1, 1
	movi r2, 2
 IsPrimeLoop:
	mul  r3, r2, r2
	sub  r4, r0, r3
	bltz r4, IsPrimeFinish
	div  r4, r0, r2
	mul  r4, r4, r2
	sub  r4, r0, r4
	addi r2, r2, 1
	bnez r4, IsPrimeLoop
	movi r1, 0
 IsPrimeFinish:
	b    lr



HWISR:
	addi r8, interruptReg, 0
	andi r9, r8, MASK_TIMER_INT
	bnez r9, TIMERISR
	br   TERMINALISR


TIMERISR:
	#clear timer 
	subi r16, r16, 1
	addi r17, r17, 1
	movi r8, 0
	nori r9, r8, MASK_TIMER_INT
	and  r58, r58, r9
	movi timerReg, 500
	RESUME


TERMINALISR:
	movi r8, 0
	nori r9, r8, MASK_TERM_INT
	and  r58, r58, r9

	movi r8, READNEXTDATA
	movi r9, TERMINALBASE
	load r10, r9, 8		
	store r8, r9, 0
	
	RESUME



SWISR:

	RESUME



