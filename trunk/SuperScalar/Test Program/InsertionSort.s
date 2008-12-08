	.file	1 "InsertionSort.c"
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
	li	$2,10			# 0xa
	sw	$2,16($fp)
	li	$2,3			# 0x3
	sw	$2,20($fp)
	li	$2,8			# 0x8
	sw	$2,24($fp)
	li	$2,7			# 0x7
	sw	$2,28($fp)
	li	$2,100			# 0x64
	sw	$2,32($fp)
	li	$2,5			# 0x5
	sw	$2,36($fp)
	li	$2,4			# 0x4
	sw	$2,40($fp)
	li	$2,74			# 0x4a
	sw	$2,44($fp)
	li	$2,2			# 0x2
	sw	$2,48($fp)
	li	$2,1			# 0x1
	sw	$2,52($fp)
	addu	$4,$fp,16
	li	$5,10			# 0xa
	jal	InsertionSort
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
	li	$4,100			# 0x64
	li	$5,90			# 0x5a
	jal	getRemainder
$L2:
	lw	$2,100($fp)
	bne	$2,$0,$L4
	j	$L3
$L4:
	lw	$2,100($fp)
	addu	$2,$2,1
	sw	$2,100($fp)
	j	$L2
$L3:
	move	$2,$0
	move	$sp,$fp
	lw	$31,108($sp)
	lw	$fp,104($sp)
	addu	$sp,$sp,112
	j	$31
	.end	main
	.align	2
	.globl	swap
	.ent	swap
swap:
	.frame	$fp,16,$31		# vars= 8, regs= 1/0, args= 0, extra= 0
	.mask	0x40000000,-8
	.fmask	0x00000000,0
	subu	$sp,$sp,16
	sw	$fp,8($sp)
	move	$fp,$sp
	sw	$4,16($fp)
	sw	$5,20($fp)
	lw	$2,16($fp)
	lw	$2,0($2)
	sw	$2,0($fp)
	lw	$3,16($fp)
	lw	$2,20($fp)
	lw	$2,0($2)
	sw	$2,0($3)
	lw	$3,20($fp)
	lw	$2,0($fp)
	sw	$2,0($3)
	move	$sp,$fp
	lw	$fp,8($sp)
	addu	$sp,$sp,16
	j	$31
	.end	swap
	.align	2
	.globl	sort
	.ent	sort
sort:
	.frame	$fp,40,$31		# vars= 16, regs= 2/0, args= 16, extra= 0
	.mask	0xc0000000,-4
	.fmask	0x00000000,0
	subu	$sp,$sp,40
	sw	$31,36($sp)
	sw	$fp,32($sp)
	move	$fp,$sp
	sw	$4,40($fp)
	sw	$5,44($fp)
	sw	$6,48($fp)
	lw	$2,44($fp)
	addu	$3,$2,1
	lw	$2,48($fp)
	slt	$2,$3,$2
	beq	$2,$0,$L6
	lw	$2,44($fp)
	sll	$3,$2,2
	lw	$2,40($fp)
	addu	$2,$3,$2
	lw	$2,0($2)
	sw	$2,16($fp)
	lw	$2,44($fp)
	addu	$2,$2,1
	sw	$2,20($fp)
	lw	$2,48($fp)
	sw	$2,24($fp)
$L8:
	lw	$2,20($fp)
	lw	$3,24($fp)
	slt	$2,$2,$3
	bne	$2,$0,$L10
	j	$L9
$L10:
	lw	$2,20($fp)
	sll	$3,$2,2
	lw	$2,40($fp)
	addu	$2,$3,$2
	lw	$3,0($2)
	lw	$2,16($fp)
	slt	$2,$2,$3
	bne	$2,$0,$L11
	lw	$2,20($fp)
	addu	$2,$2,1
	sw	$2,20($fp)
	j	$L8
$L11:
	lw	$2,20($fp)
	sll	$3,$2,2
	lw	$2,40($fp)
	addu	$4,$3,$2
	lw	$2,24($fp)
	addu	$2,$2,-1
	sw	$2,24($fp)
	sll	$3,$2,2
	lw	$2,40($fp)
	addu	$2,$3,$2
	move	$5,$2
	jal	swap
	j	$L8
$L9:
	lw	$2,20($fp)
	addu	$2,$2,-1
	sw	$2,20($fp)
	sll	$3,$2,2
	lw	$2,40($fp)
	addu	$4,$3,$2
	lw	$2,44($fp)
	sll	$3,$2,2
	lw	$2,40($fp)
	addu	$2,$3,$2
	move	$5,$2
	jal	swap
	lw	$4,40($fp)
	lw	$5,44($fp)
	lw	$6,20($fp)
	jal	sort
	lw	$4,40($fp)
	lw	$5,24($fp)
	lw	$6,48($fp)
	jal	sort
$L6:
	move	$sp,$fp
	lw	$31,36($sp)
	lw	$fp,32($sp)
	addu	$sp,$sp,40
	j	$31
	.end	sort
	.align	2
	.globl	InsertionSort
	.ent	InsertionSort
InsertionSort:
	.frame	$fp,24,$31		# vars= 16, regs= 1/0, args= 0, extra= 0
	.mask	0x40000000,-8
	.fmask	0x00000000,0
	subu	$sp,$sp,24
	sw	$fp,16($sp)
	move	$fp,$sp
	sw	$4,24($fp)
	sw	$5,28($fp)
	li	$2,1			# 0x1
	sw	$2,4($fp)
$L14:
	lw	$2,4($fp)
	lw	$3,28($fp)
	slt	$2,$2,$3
	bne	$2,$0,$L17
	j	$L13
$L17:
	lw	$2,4($fp)
	sll	$3,$2,2
	lw	$2,24($fp)
	addu	$2,$3,$2
	lw	$2,0($2)
	sw	$2,8($fp)
	lw	$2,4($fp)
	sw	$2,0($fp)
$L18:
	lw	$2,0($fp)
	blez	$2,$L19
	lw	$2,0($fp)
	sll	$3,$2,2
	lw	$2,24($fp)
	addu	$2,$3,$2
	addu	$2,$2,-4
	lw	$3,0($2)
	lw	$2,8($fp)
	slt	$2,$2,$3
	bne	$2,$0,$L21
	j	$L19
$L21:
	lw	$2,0($fp)
	sll	$3,$2,2
	lw	$2,24($fp)
	addu	$4,$3,$2
	lw	$2,0($fp)
	sll	$3,$2,2
	lw	$2,24($fp)
	addu	$2,$3,$2
	addu	$2,$2,-4
	lw	$2,0($2)
	sw	$2,0($4)
	lw	$2,0($fp)
	addu	$2,$2,-1
	sw	$2,0($fp)
	j	$L18
$L19:
	lw	$2,0($fp)
	sll	$3,$2,2
	lw	$2,24($fp)
	addu	$3,$3,$2
	lw	$2,8($fp)
	sw	$2,0($3)
	lw	$2,4($fp)
	addu	$2,$2,1
	sw	$2,4($fp)
	j	$L14
$L13:
	move	$sp,$fp
	lw	$fp,16($sp)
	addu	$sp,$sp,24
	j	$31
	.end	InsertionSort
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
$L24:
	lw	$2,8($fp)
	bgez	$2,$L26
	j	$L25
$L26:
	lw	$2,8($fp)
	lw	$3,12($fp)
	subu	$2,$2,$3
	sw	$2,8($fp)
	j	$L24
$L25:
	lw	$3,8($fp)
	lw	$2,12($fp)
	addu	$2,$3,$2
	move	$sp,$fp
	lw	$fp,0($sp)
	addu	$sp,$sp,8
	j	$31
	.end	getRemainder
