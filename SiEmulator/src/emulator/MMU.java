/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

package emulator;
import java.util.*;

/**
 *
 * @author ys8511
 */
public class MMU {
    
    private boolean instrReady;
    private int instruction;
    private int instrAddr;
    
    private boolean dataReady;
    private int data;
    private int dataAddr;
    
    private ArrayList<Interconnect> interconnects = new ArrayList<Interconnect>();
	
    
    public MMU() {
        
    }
    
    void attachInterconnect(Interconnect connection) {
		// TODO Auto-generated method stub
		this.interconnects.add(connection);		
    }
    
    public boolean getInstrReady() { return instrReady; }
    public boolean getDataReady() { return dataReady; }
    public int getInstruction() { return this.instruction; }
    public int getInstrAddr() { return this.instrAddr; }
    public int getData() { return this.data; }
    public int getDataAddr() { return this.dataAddr; }
    public boolean getCanWriteData() { return true; }
    
    public void readInstruction(int instrAddr) {
        this.instrAddr = instrAddr;
        this.instrReady = checkReadSignals(instrAddr);
        if(this.instrReady) {
            Interconnect bus = getInterconnect(dataAddr);
            this.instruction = bus.data;
        }
    }
    
    public void readData(int dataAddr) {
        this.dataAddr = dataAddr;
        this.dataReady = checkReadSignals(dataAddr);
        if(this.dataReady) {
            Interconnect bus = getInterconnect(dataAddr);
            this.data = bus.data;
        }
    }
    
    //Suppose everything can finish in one cycle
    //Add Ready Signal for extension
    public boolean checkReadSignals(int addr) {
        Interconnect bus = getInterconnect(addr);
        if(bus.address != addr) {
            bus.address = addr;
            bus.en = true;
            bus.rwbar = true;
            return false;
        } 
        return true;
        
    }
    
    public void writeData(int dataAddr, int data) {
        this.dataAddr = dataAddr;
        this.data = data;
        Interconnect bus = getInterconnect(dataAddr);
        bus.address = dataAddr;
        bus.data = data;
        bus.rwbar = false;
        bus.en = true;
    }
    
    private Interconnect getInterconnect(int addr) {
        assert(this.interconnects.size() != 0);
        Interconnect result = null; 
        
        for(Interconnect con: this.interconnects) {
            if(result == null) {
                if(con.base <= addr) {
                    result = con;
                }
            } else {
                if(con.base <= addr && con.base > result.base) {
                    //result.base = con.base;
                    result = con;
                }
            }
            
        }
        assert(result != null);
        return result;
    }
}
