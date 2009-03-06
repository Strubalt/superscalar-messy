/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

package emulator;

/**
 * 
 * This is an abstract class defining the bare bones of a Component
 * all components of the emulator much include these methods.
 */
abstract class Component {
	
	int componentType ; 
		
	public Component(int type) {
		super();
		componentType = type ;
	}
	
	abstract void advanceTime() ;
	
	/**
	 * 
	 * @param newConnection - the interconnect to attach to this component
	 */
	abstract void attachInterconnect(Interconnect connection) ;
	
}

