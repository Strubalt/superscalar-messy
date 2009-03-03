package emulator;

class WireInterconnect extends Interconnect {
		
	/**
	 * Concrete Interconnect class, modelling a simple wire as the interconnect.
	 * Multiple Components may be attached to a single wire.
	 * @param id give the interconnect a unique ID.
	 * @param components supply the list of components to be attached to the WireInterconnect.
	 */
	WireInterconnect(int id, Component[] components)
		{
		super(id, components) ;
		
		// call superclass constructor	
		// assign a default interrupt. Thes are inherited from parent
		interruptNumber = 0 ;
		isInterrupted = false ;
	
		}
	
	/**
	 * 
	 * @param newInterrupt assign the Interconnect's interrupt to this value
	 */
	void assignInterrupt(int newInterrupt)
	{
		interruptNumber = newInterrupt ;
	}
	
	/**
	 * 
	 * @param isInterrupted - set the value of the interrupt
	 */
	void setInterrupt(boolean flag)
	{
		isInterrupted = flag ;
	}
	
	boolean isInterrupted()
	{
		return isInterrupted ;
	}
	
	void writeControl(int controlData)
	{
		// does nothing
	}
	
	int readControl()
	{
		// return nothing
		return 0 ;
	}
	
	int readData()
	{
		return data ;
	}
	
	/**
	 * 
	 * @param data - data to be written to the interconnect
	 */
	void writeData(int data)
	{
		this.data = data ; 
		return ; 
	}
	
	int readAddress()
	{
		return address ;
	}
	
	/**
	 * 
	 * @param address - the address
	 */
	void writeAddress(int address)
	{
		this.address= address ;
	}
	
	/**
	 * 
	 * @param newConnection - the interconnect to attach to this interconnect
	 */
	void attachInterconnect(Interconnect newConnection)
	{
		// add another interconnect to the list of components
		Component[] newComponents = new Component[componentList.length+1] ;
		for (int i = 0 ; i < componentList.length ; i++) newComponents[i] = componentList[i] ;
		newComponents[componentList.length] = newConnection ;
		componentList = newComponents ;
	}
	
	
	void advanceProcessorTime()
	{
		for (int i = 0 ; i < componentList.length ; i++)
		if (componentList[i].componentType == 0) componentList[i].advanceTime() ;
	}
	
	void advanceMemoryTime()
	{
		for (int i = 0 ; i < componentList.length ; i++)
		if (componentList[i].componentType == 1) componentList[i].advanceTime() ;
	}
	
	
	void advanceTerminalTime()
	{	
		for (int i = 0 ; i < componentList.length ; i++)
		if (componentList[i].componentType == 2) componentList[i].advanceTime() ;
	}
	
	void advanceInterconnectTime()
	{
		for (int i = 0 ; i < componentList.length ; i++)
		if (componentList[i].componentType == 3) componentList[i].advanceTime() ;
	}
	
	/**
	 *  Advance time for the interconnect. For a wire, this does nothing
	 *  except advance time for any other interconnects attached to it
	 * 
	 */ 
	void advanceTime()
	{
		advanceInterconnectTime() ;
	}



// end class
}
