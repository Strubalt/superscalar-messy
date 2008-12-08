
int gcd(int input1, int input2);
int getRemainder(int dividend, int divisor);
int recursiveGCD(int largeNum, int smallNum);

int main(void)
{	
	int a[10],b[10],c,d;
	c=74;
	d=48;
	//d = gcd(a,c);
	int largeNum,smallNum;
	if (c > d)
	{
	    largeNum=c;
		smallNum=d;
	}
    else /*if input1 <= input2 */
	{
		largeNum=d;
		smallNum=c;
	}

	
	int remainder = getRemainder(largeNum,smallNum);
	while(remainder){
		largeNum=smallNum;
		smallNum=remainder;
		remainder=getRemainder(largeNum,smallNum);
	}
	d=smallNum;
	a[0] = 100;
	a[1] = 100;
	a[2] = 100;
	a[3] = 100;
	a[4] = 100;
	a[5] = 100;
	a[6] = 100;
	a[7] = 100;
	a[8] = 100;
	a[9] = 100;
	b[0] = 100;
	b[1] = 100;
	b[2] = 100;
	b[3] = 100;
	b[4] = 100;
	b[5] = 100;
	b[6] = 100;
	b[7] = 100;
	b[8] = 100;
	b[9] = 100;
	

	

	while(1){ c+=1;}
	/* Define all variables. */
	/*int input1[100], input2[100], greatest_common_divisor;
	int i;
	aa = 3;
	cc = aa+cc;
	int gcdresult[100];
	for(i=0; i<100; i++)
	{
		input1[i] = 1000+100*i;
		input2[i] = 10+10*i;
	}
	for(i = 0; i<100; i++)
			gcdresult[i] = gcd(input1[i], input2[i]);*/

	return 0;
}


int gcd(int input1, int input2)
{
    
    /* Compare two numbers, and get the smallest number.*/
    if (input1 > input2)
	{
	    return recursiveGCD(input1,input2);
	}
    else /*if input1 <= input2 */
	{
		return recursiveGCD(input2,input1);
        
	}
	
}

int recursiveGCD(int largeNum, int smallNum)
{
	int remainder = getRemainder(largeNum,smallNum);
	if(0==remainder){
		return smallNum;
	}
	else{
		return recursiveGCD(smallNum,remainder);
	}
}

int getRemainder(int dividend, int divisor)
{
	while(dividend>=0)
	{
		dividend -= divisor;
	}
	return dividend+divisor;
}


