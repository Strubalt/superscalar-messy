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
    
    private static class MemoryOperation {
        public int address;
        public int data;
        public boolean rwbar;
        public boolean isInstruction;
        public MemoryOperation(int addr, int data, boolean rwbar, boolean isInstruction) {
            this.address = addr;
            this.data = data;
            this.rwbar = rwbar;
            this.isInstruction = isInstruction;
        }
        
    }
    
    //private ArrayList<Interconnect> interconnects = new ArrayList<Interconnect>();
    private Interconnect bus;
    private ArrayList<MemoryOperation> buffers = new ArrayList<MemoryOperation>();
    
    public MMU() {
        instrCache = new Cache(Config.cacheNumSet, Config.cacheNumWay);
        
    }
    
    private int exClkFactor=0;
    void advanceTime() {
        if(buffers.size() == 0) {
            return;
        }
        MemoryOperation op = buffers.get(0);
        if(bus.en && bus.address == op.address) {
            if(bus.ready) { //last operation has finished
                buffers.remove(0);
                bus.en = false;
                if(bus.rwbar) {
                    if(op.isInstruction) {
                        this.instrAddr = bus.address;
                        this.instruction = bus.data;
                        this.instrReady = true;
                        instrCache.write(instrAddr, instruction);
                    } else {
                        this.dataAddr = bus.address;
                        this.data = bus.data;
                        this.dataReady = true;
                    }
                }
            } else {
                //wait
            }
        } else {
            bus.address = op.address;
            bus.en = true;
            bus.data = op.data;
            bus.ready = false;
            bus.rwbar = op.rwbar;
            return;
        }
        if(buffers.size() == 0) {
            return;
        }
        if(!bus.en) {
            op = buffers.get(0);
            bus.address = op.address;
            bus.en = true;
            bus.data = op.data;
            bus.ready = false;
            bus.rwbar = op.rwbar;
        }
        //Interconnect bus = getInterconnect(addr);
        /*
        if(bus.en && bus.ready && !bus.rwbar) {
            bus.en = false;
            bus.ready = false;
        }
         */
        
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
    
    private void addOperation(int addr, int data, boolean rwbar, boolean isInstruction) {
        for(MemoryOperation op : this.buffers) {
            if(op.address == addr && op.rwbar == rwbar && op.data == data) {
                return;
            }
        }
        this.buffers.add(new MemoryOperation(addr,data,rwbar,isInstruction));
    }
    
    private boolean containsWriteOperation(int addr) {
        for(MemoryOperation op : this.buffers) {
            if(op.address == addr && !op.rwbar) {
                return true;
            }
        }
        return false;
    }
    
    private int getWriteData(int addr) {
        for(MemoryOperation op : this.buffers) {
            if(op.address == addr && !op.rwbar) {
                return op.data;
            }
        }
        assert(true);
        return 0xf0000000;
    }
    
    public void readInstruction(int instrAddr) {
        if(this.instrReady && this.instrAddr == instrAddr) {
            return;
        } 
        this.instrAddr = instrAddr;
        
        if(instrCache.exist(instrAddr)) {
            this.instrReady = true;
            this.instruction = instrCache.read(instrAddr);
        } else {
            this.instrReady = false;
            //this.instrReady = checkReadSignals(instrAddr);
            addOperation(instrAddr, 0, true, true);
            /*
            if(this.instrReady) {
                this.instruction = bus.data;
                instrCache.write(instrAddr, instruction);
            }
             */ 
        }
        
        
    }
    
    public void cancelInstructionRead() {
        int idx=0;
        for(idx=0; idx<buffers.size(); idx++) {
            if(this.buffers.get(idx).isInstruction) {
                this.buffers.remove(idx);
                return;
            }
        }
        /*
        if(this.isInstr && this.bus.en) {
            this.bus.en = false;
        }*/
    }
    
    public void readData(int dataAddr) {
        if(this.dataReady && this.dataAddr == dataAddr) {
            return;
        } 
        this.dataReady = false;
        this.dataAddr = dataAddr;
        addOperation(dataAddr, 0, true, false);
        /*
        this.dataReady = checkReadSignals(dataAddr, false);
        if(this.dataReady) {
            this.data = bus.data;
        }
         */
    }
    
    
    //Suppose everything can finish in one cycle
    //Add Ready Signal for extension
    private boolean checkReadSignals(int addr) {
       
        if(bus.en && bus.address != addr) {
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
            
            return false;
        }
    }
    
    public boolean canWriteData() {
        if(bus.en && !bus.ready && !bus.rwbar)
            return false;
        return true;
    }
    
    public void writeData(int dataAddr, int data) {
        addOperation(dataAddr, data, false, false);
        /*
        this.dataAddr = dataAddr;
        this.data = data;
        this.isInstr = false;
        bus.address = dataAddr;
        bus.data = data;
        bus.rwbar = false;
        bus.en = true;
        bus.ready = false;
         */
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