	.file	1 "TotalTest.c"
	.text
	.align	2
	.globl	main
	.ent	main
main:
	.frame	$fp,208,$31		# vars= 176, regs= 3/0, args= 16, extra= 0
	.mask	0xc0010000,-8
	.fmask	0x00000000,0
	subu	$sp,$sp,208
	sw	$31,200($sp)
	sw	$fp,196($sp)
	sw	$16,192($sp)
	move	$fp,$sp
	jal	__main
	sw	$0,184($fp)
	sw	$0,176($fp)
$L2:
	lw	$2,176($fp)
	slt	$2,$2,20
	bne	$2,$0,$L5
	j	$L3
$L5:
	lw	$2,176($fp)
	sll	$3,$2,2
	addu	$2,$fp,16
	addu	$3,$3,$2
	lw	$2,176($fp)
	addu	$2,$2,75
	sw	$2,0($3)
	lw	$2,176($fp)
	sll	$3,$2,2
	addu	$2,$fp,16
	addu	$2,$3,$2
	addu	$3,$2,80
	lw	$2,176($fp)
	addu	$2,$2,90
	sw	$2,0($3)
	lw	$2,176($fp)
	addu	$2,$2,1
	sw	$2,176($fp)
	j	$L2
$L3:
	addu	$4,$fp,16
	li	$5,20			# 0x14
	jal	InsertionSort
	addu	$2,$fp,96
	move	$4,$2
	li	$5,20			# 0x14
	jal	InsertionSort
	addu	$4,$fp,16
	li	$5,20			# 0x14
	jal	InsertionSort
	addu	$2,$fp,96
	move	$4,$2
	li	$5,20			# 0x14
	jal	InsertionSort
	sw	$0,176($fp)
$L6:
	lw	$2,176($fp)
	slt	$2,$2,20
	bne	$2,$0,$L9
	j	$L7
$L9:
	lw	$2,176($fp)
	sll	$3,$2,2
	addu	$2,$fp,16
	addu	$16,$3,$2
	lw	$2,176($fp)
	sll	$3,$2,2
	addu	$2,$fp,16
	addu	$4,$3,$2
	lw	$2,176($fp)
	sll	$3,$2,2
	addu	$2,$fp,16
	addu	$2,$3,$2
	addu	$2,$2,80
	lw	$4,0($4)
	lw	$5,0($2)
	jal	gcd
	sw	$2,0($16)
	lw	$2,176($fp)
	addu	$2,$2,1
	sw	$2,176($fp)
	j	$L6
$L7:
	addu	$4,$fp,16
	li	$5,20			# 0x14
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
	sw	$2,96($fp)
	li	$2,100			# 0x64
	sw	$2,100($fp)
	li	$2,100			# 0x64
	sw	$2,104($fp)
	li	$2,100			# 0x64
	sw	$2,108($fp)
	li	$2,100			# 0x64
	sw	$2,112($fp)
	li	$2,100			# 0x64
	sw	$2,116($fp)
	li	$2,100			# 0x64
	sw	$2,120($fp)
	li	$2,100			# 0x64
	sw	$2,124($fp)
	li	$2,100			# 0x64
	sw	$2,128($fp)
	li	$2,100			# 0x64
	sw	$2,132($fp)
$L10:
	lw	$2,184($fp)
	bne	$2,$0,$L12
	j	$L11
$L12:
	lw	$2,184($fp)
	addu	$2,$2,1
	sw	$2,184($fp)
	j	$L10
$L11:
	move	$2,$0
	move	$sp,$fp
	lw	$31,200($sp)
	lw	$fp,196($sp)
	lw	$16,192($sp)
	addu	$sp,$sp,208
	j	$31
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
	beq	$2,$0,$L14
	lw	$4,32($fp)
	lw	$5,36($fp)
	jal	recursiveGCD
	sw	$2,16($fp)
	j	$L13
$L14:
	lw	$4,36($fp)
	lw	$5,32($fp)
	jal	recursiveGCD
	sw	$2,16($fp)
$L13:
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
	bne	$2,$0,$L17
	lw	$2,36($fp)
	sw	$2,20($fp)
	j	$L16
$L17:
	lw	$4,36($fp)
	lw	$5,16($fp)
	jal	recursiveGCD
	sw	$2,20($fp)
$L16:
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
$L20:
	lw	$2,8($fp)
	bgez	$2,$L22
	j	$L21
$L22:
	lw	$2,8($fp)
	lw	$3,12($fp)
	subu	$2,$2,$3
	sw	$2,8($fp)
	j	$L20
$L21:
	lw	$3,8($fp)
	lw	$2,12($fp)
	addu	$2,$3,$2
	move	$sp,$fp
	lw	$fp,0($sp)
	addu	$sp,$sp,8
	j	$31
	.end	getRemainder
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
	beq	$2,$0,$L24
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
$L26:
	lw	$2,20($fp)
	lw	$3,24($fp)
	slt	$2,$2,$3
	bne	$2,$0,$L28
	j	$L27
$L28:
	lw	$2,20($fp)
	sll	$3,$2,2
	lw	$2,40($fp)
	addu	$2,$3,$2
	lw	$3,0($2)
	lw	$2,16($fp)
	slt	$2,$2,$3
	bne	$2,$0,$L29
	lw	$2,20($fp)
	addu	$2,$2,1
	sw	$2,20($fp)
	j	$L26
$L29:
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
	j	$L26
$L27:
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
$L24:
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
$L32:
	lw	$2,4($fp)
	lw	$3,28($fp)
	slt	$2,$2,$3
	bne	$2,$0,$L35
	j	$L31
$L35:
	lw	$2,4($fp)
	sll	$3,$2,2
	lw	$2,24($fp)
	addu	$2,$3,$2
	lw	$2,0($2)
	sw	$2,8($fp)
	lw	$2,4($fp)
	sw	$2,0($fp)
$L36:
	lw	$2,0($fp)
	blez	$2,$L37
	lw	$2,0($fp)
	sll	$3,$2,2
	lw	$2,24($fp)
	addu	$2,$3,$2
	addu	$2,$2,-4
	lw	$3,0($2)
	lw	$2,8($fp)
	slt	$2,$2,$3
	bne	$2,$0,$L39
	j	$L37
$L39:
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
	j	$L36
$L37:
	lw	$2,0($fp)
	sll	$3,$2,2
	lw	$2,24($fp)
	addu	$3,$3,$2
	lw	$2,8($fp)
	sw	$2,0($3)
	lw	$2,4($fp)
	addu	$2,$2,1
	sw	$2,4($fp)
	j	$L32
$L31:
	move	$sp,$fp
	lw	$fp,16($sp)
	addu	$sp,$sp,24
	j	$31
	.end	InsertionSort
