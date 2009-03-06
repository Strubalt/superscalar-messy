/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

package emulator;


/**
 * Abstract Interconnect class
 * All concrete interconnects must extend this and use the local variables to store state.
 */
abstract class Interconnect extends Component {
	

	int id ;
	int interruptNumber ;
	
	// interconnect base address. If not applicable, can initialise to -1
	int base ;
	
	// Model an interconnect as a one word buffer
	int data ; 
	int address ;
	
	// enable wire for a Component
	boolean en ;
	// read/write_bar for an attached Component
	boolean rwbar ;

	boolean isInterrupted ;
	
	Component[] componentList ;

	
/**
 * 
 * @param id give the interconnect a unique ID. Automatically updated if none specified.
 * @param a,b give the two components to which the interconnect is attached
 * 
 */
	
Interconnect(int id, Component[] components )
{
	super(3) ;
	this.id = id ;
        base = -1;
        address = -1;
	componentList = components ;
	for (int i = 0 ; i < componentList.length ; i++) componentList[i].attachInterconnect(this) ;
}
	
void setBase(int newBase)
{
	base = newBase ;
}

int getBase()
{
	return base ;
}

/**
 * 
 * @param newInterrupt assign the Interconnect's interrupt to this value
 */
abstract void assignInterrupt(int newInterrupt) ;

abstract void setInterrupt(boolean flag) ;

abstract boolean isInterrupted() ;

abstract void writeControl(int controlData) ;

abstract int readControl() ;

abstract int readData() ;

abstract void writeData(int data) ;

abstract void writeAddress(int address) ;

abstract int readAddress() ;

abstract void advanceMemoryTime() ;

abstract void advanceProcessorTime() ;

abstract void advanceTerminalTime() ;

abstract void advanceTime() ;

// end class
}