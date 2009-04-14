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
    private Cache instrCache;
    
    private boolean dataReady;
    private int data;
    private int dataAddr;
    
    //private ArrayList<Interconnect> interconnects = new ArrayList<Interconnect>();
    private Interconnect bus;
	
    
    public MMU() {
        instrCache = new Cache(16,1);
       
    }
    
    void advanceTime() {
        //Interconnect bus = getInterconnect(addr);
        if(bus.en && bus.ready && !bus.rwbar) {
            bus.en = false;
            bus.ready = false;
        }
    }
    
    void attachInterconnect(Interconnect connection) {
		// TODO Auto-generated method stub
	this.bus = connection;
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
        if(instrCache.exist(instrAddr)) {
            this.instrReady = true;
            this.instruction = instrCache.read(instrAddr);
        } else {
            this.instrReady = checkReadSignals(instrAddr, true);
            if(this.instrReady) {
                this.instruction = bus.data;
                instrCache.write(instrAddr, instruction);
            }
        }
        
        
    }
    
    public void readData(int dataAddr) {
        if(this.dataReady && this.dataAddr == dataAddr) {
            return;
        }
        this.dataAddr = dataAddr;
        this.dataReady = checkReadSignals(dataAddr, false);
        if(this.dataReady) {
            this.data = bus.data;
        }
    }
    
    private boolean isInstr;
    //Suppose everything can finish in one cycle
    //Add Ready Signal for extension
    private boolean checkReadSignals(int addr, boolean isInstr) {
        if(bus.en && bus.address != addr && this.isInstr == isInstr) {
                bus.en = false;
        }
        if(bus.en) {
            if(bus.ready && bus.address == addr) {
                bus.ready = false;
                bus.en = false;
                return true;  
            }
            
            return false;
        } else {
            bus.address = addr;
            bus.en = true;
            bus.ready = false;
            bus.rwbar = true;
            this.isInstr = isInstr;
            return false;
        }
    }
    
    public void writeData(int dataAddr, int data) {
        this.dataAddr = dataAddr;
        this.data = data;
        this.isInstr = false;
        bus.address = dataAddr;
        bus.data = data;
        bus.rwbar = false;
        bus.en = true;
        bus.ready = false;
    }

/*
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
 */ 
}