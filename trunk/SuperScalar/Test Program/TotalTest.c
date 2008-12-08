
#define size 20

int gcd(int input1, int input2);
int getRemainder(int dividend, int divisor);
int recursiveGCD(int largeNum, int smallNum);

void swap(int *a, int *b);
void InsertionSort( int A[ ], int N );

int main(void)
{	
	int a[size],b[size];
	int idx,j;
	int sum=0;
	for(idx = 0; idx <size; idx++)
	{
		a[idx]=idx+75;
		b[idx]=idx+90;
	}
	InsertionSort(a,size);
	InsertionSort(b,size);
	InsertionSort(a,size);
	InsertionSort(b,size);
	for(idx = 0; idx <size; idx++)
	{
		a[idx] = gcd(a[idx],b[idx]);
	}
	InsertionSort(a,size);

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
	while(sum){ sum+=1;}
	
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


void swap(int *a, int *b)
{
  int t=*a; *a=*b; *b=t;
}
void sort(int arr[], int beg, int end)
{
  if (end > beg + 1)
  {
    int piv = arr[beg], l = beg + 1, r = end;
    while (l < r)
    {
      if (arr[l] <= piv)
        l++;
      else
        swap(&arr[l], &arr[--r]);
    }
    swap(&arr[--l], &arr[beg]);
    sort(arr, beg, l);
    sort(arr, r, end);
  }
}

void
InsertionSort( int A[ ], int N )
{
    int j, P;
    int Tmp;

	for( P = 1; P < N; P++ )
    {
		Tmp = A[ P ];
		for( j = P; j > 0 && A[ j - 1 ] > Tmp; j-- )
			A[ j ] = A[ j - 1 ];
		A[ j ] = Tmp;
    }
}


