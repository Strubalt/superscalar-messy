
void swap(int *a, int *b);
void sort(int arr[], int beg, int end);
int getRemainder(int dividend, int divisor);

int main(void)
{	
	int a[10],b[10];
	int idx;
	int sum=0;
	/*for(idx=0; idx<10; ++idx)
	{
		a[idx]=5+idx;
		if(getRemainder(idx,2)==0)
			a[idx]=idx-5;
		if(getRemainder(idx,3)==0)
			a[idx]=idx+2;
		
	}*/
	a[0]=10;
	a[1]=3;
	a[2]=8;
	a[3]=7;
	a[4]=100;
	a[5]=5;
	a[6]=4;
	a[7]=74;
	a[8]=2;
	a[9]=1;
	sort(a,0,10);
	
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
	getRemainder(100,90);
	while(sum){ sum+=1;}
	
	return 0;
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


int getRemainder(int dividend, int divisor)
{
	while(dividend>=0)
	{
		dividend -= divisor;
	}
	return dividend+divisor;
}

