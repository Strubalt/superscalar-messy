	.file	1 "InlineGcd.c"
	.text
	.align	2
	.globl	main
	.ent	main
main:
	.frame	$fp,128,$31		# vars= 104, regs= 2/0, args= 16, extra= 0
	.mask	0xc0000000,-4
	.fmask	0x00000000,0
	subu	$sp,$sp,128
	sw	$31,124($sp)
	sw	$fp,120($sp)
	move	$fp,$sp
	jal	__main
	li	$2,74			# 0x4a
	sw	$2,96($fp)
	li	$2,48			# 0x30
	sw	$2,100($fp)
	lw	$3,96($fp)
	lw	$2,100($fp)
	slt	$2,$2,$3
	beq	$2,$0,$L2
	lw	$2,96($fp)
	sw	$2,104($fp)
	lw	$2,100($fp)
	sw	$2,108($fp)
	j	$L3
$L2:
	lw	$2,100($fp)
	sw	$2,104($fp)
	lw	$2,96($fp)
	sw	$2,108($fp)
$L3:
	lw	$4,104($fp)
	lw	$5,108($fp)
	jal	getRemainder
	sw	$2,112($fp)
$L4:
	lw	$2,112($fp)
	bne	$2,$0,$L6
	j	$L5
$L6:
	lw	$2,108($fp)
	sw	$2,104($fp)
	lw	$2,112($fp)
	sw	$2,108($fp)
	lw	$4,104($fp)
	lw	$5,108($fp)
	jal	getRemainder
	sw	$2,112($fp)
	j	$L4
$L5:
	lw	$2,108($fp)
	sw	$2,100($fp)
	li	$2,100			# 0x64
	sw	$2,16($fp)
	li	$2,100			# 0x64
	sw	$2,20($fp)
	li	$2,100			# 0x64
	sw	$2,24($fp)
	li	$2,100			# 0x64
	sw	$2,28($fp)
	li	$2,100			# 0x64
	sw	$2,32($fp)
	li	$2,100			# 0x64
	sw	$2,36($fp)
	li	$2,100			# 0x64
	sw	$2,40($fp)
	li	$2,100			# 0x64
	sw	$2,44($fp)
	li	$2,100			# 0x64
	sw	$2,48($fp)
	li	$2,100			# 0x64
	sw	$2,52($fp)
	li	$2,100			# 0x64
	sw	$2,56($fp)
	li	$2,100			# 0x64
	sw	$2,60($fp)
	li	$2,100			# 0x64
	sw	$2,64($fp)
	li	$2,100			# 0x64
	sw	$2,68($fp)
	li	$2,100			# 0x64
	sw	$2,72($fp)
	li	$2,100			# 0x64
	sw	$2,76($fp)
	li	$2,100			# 0x64
	sw	$2,80($fp)
	li	$2,100			# 0x64
	sw	$2,84($fp)
	li	$2,100			# 0x64
	sw	$2,88($fp)
	li	$2,100			# 0x64
	sw	$2,92($fp)
$L7:
	lw	$2,96($fp)
	addu	$2,$2,1
	sw	$2,96($fp)
	j	$L7
	.end	main
	.align	2
	.globl	gcd
	.ent	gcd
gcd:
	.frame	$fp,32,$31		# vars= 8, regs= 2/0, args= 16, extra= 0
	.mask	0xc0000000,-4
	.fmask	0x00000000,0
	subu	$sp,$sp,32
	sw	$31,28($sp)
	sw	$fp,24($sp)
	move	$fp,$sp
	sw	$4,32($fp)
	sw	$5,36($fp)
	lw	$2,32($fp)
	lw	$3,36($fp)
	slt	$2,$3,$2
	beq	$2,$0,$L11
	lw	$4,32($fp)
	lw	$5,36($fp)
	jal	recursiveGCD
	sw	$2,16($fp)
	j	$L10
$L11:
	lw	$4,36($fp)
	lw	$5,32($fp)
	jal	recursiveGCD
	sw	$2,16($fp)
$L10:
	lw	$2,16($fp)
	move	$sp,$fp
	lw	$31,28($sp)
	lw	$fp,24($sp)
	addu	$sp,$sp,32
	j	$31
	.end	gcd
	.align	2
	.globl	recursiveGCD
	.ent	recursiveGCD
recursiveGCD:
	.frame	$fp,32,$31		# vars= 8, regs= 2/0, args= 16, extra= 0
	.mask	0xc0000000,-4
	.fmask	0x00000000,0
	subu	$sp,$sp,32
	sw	$31,28($sp)
	sw	$fp,24($sp)
	move	$fp,$sp
	sw	$4,32($fp)
	sw	$5,36($fp)
	lw	$4,32($fp)
	lw	$5,36($fp)
	jal	getRemainder
	sw	$2,16($fp)
	lw	$2,16($fp)
	bne	$2,$0,$L14
	lw	$2,36($fp)
	sw	$2,20($fp)
	j	$L13
$L14:
	lw	$4,36($fp)
	lw	$5,16($fp)
	jal	recursiveGCD
	sw	$2,20($fp)
$L13:
	lw	$2,20($fp)
	move	$sp,$fp
	lw	$31,28($sp)
	lw	$fp,24($sp)
	addu	$sp,$sp,32
	j	$31
	.end	recursiveGCD
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
$L17:
	lw	$2,8($fp)
	bgez	$2,$L19
	j	$L18
$L19:
	lw	$2,8($fp)
	lw	$3,12($fp)
	subu	$2,$2,$3
	sw	$2,8($fp)
	j	$L17
$L18:
	lw	$3,8($fp)
	lw	$2,12($fp)
	addu	$2,$3,$2
	move	$sp,$fp
	lw	$fp,0($sp)
	addu	$sp,$sp,8
	j	$31
	.end	getRemainder
