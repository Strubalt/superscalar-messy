	.file	1 "SimpleLoop.c"
	.text
	.align	2
	.globl	main
	.ent	main
main:
	.frame	$fp,112,$31		# vars= 88, regs= 2/0, args= 16, extra= 0
	.mask	0xc0000000,-4
	.fmask	0x00000000,0
	subu	$sp,$sp,112
	sw	$31,108($sp)
	sw	$fp,104($sp)
	move	$fp,$sp			
	jal	__main
	sw	$0,100($fp)
						#initial values
	li	$t1,100
	sw	$t1,16($fp)
	li	$t1,90
	sw	$t1,20($fp)
	li	$t1,80
	sw	$t1,24($fp)
	li	$t1,70
	sw	$t1,28($fp)
	li	$t1,60
	sw	$t1,32($fp)
	li	$t1,50
	sw	$t1,36($fp)
	li	$t1,40
	sw	$t1,40($fp)
	li	$t1,30
	sw	$t1,44($fp)
	li	$t1,20
	sw	$t1,48($fp)
	li	$t1,10
	sw	$t1,52($fp)
	li	$t1,-100
	sw	$t1,56($fp)
	li	$t1,-90
	sw	$t1,60($fp)
	li	$t1,-80
	sw	$t1,64($fp)
	li	$t1,-70
	sw	$t1,68($fp)
	li	$t1,-60
	sw	$t1,72($fp)
	li	$t1,-50
	sw	$t1,76($fp)
	li	$t1,-40
	sw	$t1,80($fp)
	li	$t1,-30
	sw	$t1,84($fp)
	li	$t1,-20
	sw	$t1,88($fp)
	li	$t1,-10
	sw	$t1,92($fp)
							# sw	$0,96($fp)
	addu $t1,$0,$0			# idx = 0; $t1, 96($fp)
$L2:
							#lw	$2,96($fp)
	slt	$t2,$t1,10			# if idx < 10 then jump out of loop
	bne	$t2,$0,$L5
	j	$L3
$L5:
							#lw	$2,96($fp)
	sll	$t2,$t1,2
	addu	$t3,$fp,16		
	addu	$t4,$t2,$t3
							#addu	$2,$t2,$t3
	addu	$2,$t4,40
	lw	$3,0($t4)
	lw	$2,0($2)
	addu	$2,$3,$2
	sw	$2,0($t4)
	addu	$t1,$t1,1
	j	$L2
$L3:
	li	$4,100			# 0x64
	li	$5,90			# 0x5a
	jal	getRemainder
$L6:
	lw	$2,100($fp)
	bne	$2,$0,$L8
	j	$L7
$L8:
	lw	$2,100($fp)
	addu	$2,$2,1
	sw	$2,100($fp)
	j	$L6
$L7:
	move	$2,$0
	move	$sp,$fp
	lw	$31,108($sp)
	lw	$fp,104($sp)
	addu	$sp,$sp,112
	j	$31
	.end	main
	.align	2
	.globl	getRemainder
	.ent	getRemainder
getRemainder:
	.frame	$fp,8,$31		# vars= 0, regs= 1/0, args= 0, extra= 0
	.mask	0x40000000,-8
	.fmask	0x00000000,0
	subu	$sp,$sp,8
	sw	$fp,0($sp)
	move	$fp,$sp
	sw	$4,8($fp)
	sw	$5,12($fp)
$L10:
	lw	$2,8($fp)
	bgez	$2,$L12
	j	$L11
$L12:
	lw	$2,8($fp)
	lw	$3,12($fp)
	subu	$2,$2,$3
	sw	$2,8($fp)
	j	$L10
$L11:
	lw	$3,8($fp)
	lw	$2,12($fp)
	addu	$2,$3,$2
	move	$sp,$fp
	lw	$fp,0($sp)
	addu	$sp,$sp,8
	j	$31
	.end	getRemainder
