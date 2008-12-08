
#define size 10

int getRemainder(int dividend, int divisor);


int main(void)
{	
	int a[size],b[size];
	int idx;
	int sum=0;
	/*for(idx=0; idx<size; ++idx)
	{
		a[idx]=5+idx;
		b[idx]=100-idx;
	}*/
	
	for(idx=0; idx<size; ++idx)
	{
		a[idx]=a[idx]+b[idx];
	}

	
	getRemainder(100,90);
	while(sum){ sum+=1;}
	
	return 0;

}


int getRemainder(int dividend, int divisor)
{
	while(dividend>=0)
	{
		dividend -= divisor;
	}
	return dividend+divisor;
}
