package emulator;
import java.lang.*;
import java.util.*;

public class Terminal extends Component{

    static class ReadThread extends Thread {
         
        public ArrayList<Integer> buffer = new ArrayList<Integer>();
        String newline = System.getProperty("line.separator");
        private boolean isEnable;
        private int bufferSize;
        
        public void run() {
            
            while(this.isEnable) {
                try {
                    int input = System.in.read();
                    addToBuffer(input);
                    if(!newline.equals((char)input))
                        input = System.in.read();
                    
                } catch(Exception e) {

                }
            }
        }
        
        public void enable(boolean isEnable) {
            this.isEnable = isEnable;
        }
        
        public void setBufferSize(int size) {
            this.bufferSize = size;
        }
        
        public int getBufferSize() {
            return this.bufferSize;
        }
        
        public synchronized void addToBuffer(int data) {
            
            this.buffer.add(new Integer(data));
            
        }
        
        public synchronized int readBuffer() {
            assert(!buffer.isEmpty());
            int result = buffer.get(0).intValue();
            this.buffer.remove(0);
            return result;
        }
        
        public boolean isEmpty() {
            return this.buffer.isEmpty();
        }
     }
    
    final int controlReg = 0;
    final int outDataReg = 1;
    final int inDataReg = 2;

    private Interconnect bus;
    int id, baseAddr, terminalNumber;
    Terminal.ReadThread readThread;
    int registers[];

    public Terminal(int id, int base, int terminalNumber) {
            super(2);
            this.id = id;
            this.baseAddr = base;
            this.terminalNumber = terminalNumber;
            registers = new int[3];
            readThread = new Terminal.ReadThread();
            readThread.enable(true);
            readThread.setBufferSize(32);
            readThread.start();
    }

    @Override
    void attachInterconnect(Interconnect connection) {
        this.bus = connection;
    }

    @Override
    void advanceTime() {
        assert(bus != null);
        int offset = bus.address-this.baseAddr;
        if(!bus.ready && bus.en && offset>=0 && offset<=8) {
            if(bus.rwbar) {
                bus.data = this.registers[offset/4];
            } else {
                this.registers[offset/4] = bus.data;
                if(offset/4 == controlReg) {
                    execute();
                }
            }
            bus.ready = true;
        }
        
        
    }
    
    final int MSK_OUT_BUFFER_FULL = 1;          //bit 0
    final int MSK_IN_BUFFER_NOT_EMPTY = 1 << 1; //bit 1
    final int MSK_WRITE_DATA = 1 << 16;         //bit 16
    final int MSK_READ_NEXT_DATA = 1 << 17;     //bit 17
    final int MSK_OUT_FORMAT = 1 << 18;         //bit 18
    final int MSK_IN_FORMAT = 1 << 19;          //bit 19
    final int FORMAT_INT = 0;
    final int FORMAT_ASCII = 1;

    private int outputFormat() {
        if(getControlRegBit(MSK_OUT_FORMAT)) {
            return FORMAT_ASCII;
        } else {
            return FORMAT_INT;
        }
    }

    private int inputFormat() {
        if(getControlRegBit(MSK_IN_FORMAT)) {
            return FORMAT_ASCII;
        } else {
            return FORMAT_INT;
        }
       
    }

   
    private boolean requireWriteData() {
        return getControlRegBit(MSK_WRITE_DATA);
    }
    
    private boolean getControlRegBit(int mask) {
        return ((this.registers[controlReg] & mask) != 0);
    }
    private void setControlRegBit(int mask) {
        this.registers[controlReg] = this.registers[controlReg] | mask;
    }
    
    private void resetControlRegBit(int mask) {
        this.registers[controlReg] = this.registers[controlReg] & (~mask);
    }

    
    private void prepareReadNextData() {
        //clear READ_NEXT_DATA so it won't redo this function again
        resetControlRegBit(MSK_READ_NEXT_DATA);
        resetControlRegBit(MSK_IN_BUFFER_NOT_EMPTY);
        
    }

    private boolean requireReadNextData() {
        return getControlRegBit(MSK_READ_NEXT_DATA);
        /*
        if((this.registers[controlReg] & MSK_READ_NEXT_DATA) != 0) {
            return true;
        }
        return false;
        */
    }
    
    private static class OutputData {
        public int Data, Format;
        
        public OutputData(int data, int format) {
            this.Data = data;
            this.Format = format;
        }
    }
    
    public ArrayList<OutputData> outputBuffer = new ArrayList<OutputData>();
    
    private void writeData() {
        /* 
        if(outputFormat() == FORMAT_INT) {
            System.out.print(this.registers[outDataReg]);
        } else {
            System.out.print((char)this.registers[outDataReg]);
        }
         */
        OutputData data = new OutputData(this.registers[outDataReg], outputFormat());
        outputBuffer.add(data);
        outputDataFromBuffer();
        resetControlRegBit(MSK_WRITE_DATA);
    }
    
    private void outputDataFromBuffer() {
        if(outputBuffer.size() != 0) {
            outputDataToTerminal(outputBuffer.get(0));
            outputBuffer.remove(0);
        }
        //output buffer no longer full
        this.resetControlRegBit(MSK_OUT_BUFFER_FULL);
    }
    
    private void outputDataToTerminal(OutputData data) {
        if(FORMAT_INT == data.Format) {
            System.out.print(data.Data);
        } else {
            System.out.print((char)data.Data);
        }
    }

    private void execute(){

        if(requireWriteData()) {
            writeData();
            //return;
        }
        if(requireReadNextData()) {
            prepareReadNextData();
            //return;
        }
    }



    int read(int address) throws java.lang.ArrayIndexOutOfBoundsException{
            int offset = address-this.baseAddr;
            if(offset>=0 && offset<=8) {
                    return this.registers[offset/4];
            }
            throw new java.lang.ArrayIndexOutOfBoundsException();

    }

    void write(int address, int data)  throws java.lang.ArrayIndexOutOfBoundsException{
            int offset = address-this.baseAddr;
            if(offset>=0 && offset<=8) {
                    this.registers[offset/4] = data;
            }
            else {
                    throw new java.lang.ArrayIndexOutOfBoundsException();
            }
    }
}