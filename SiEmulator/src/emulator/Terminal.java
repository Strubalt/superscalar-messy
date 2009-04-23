package emulator;
//import java.lang.*;
import java.util.*;

class Terminal extends Component{

    static class ReadThread extends Thread {
         
        public ArrayList<Integer> buffer = new ArrayList<Integer>();
        String newline = System.getProperty("line.separator");
        private boolean isEnable;
        private int bufferSize = 1;
        
        public void run() {
            int count;
            while(this.isEnable) {
                
                try {
                    count = System.in.available();
                    if(0 != count) {
                        byte data[] = new byte[count];
                        count = System.in.read(data);
                        
                        for(int i=0; i<count; ++i) {
                            //int input = System.in.read();
                            addToBuffer(data[i]);
                        }
                        
                        //if(!newline.equals((char)input))
                        //    input = System.in.read();
                        
                    }
                    
                    
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
    private int outputBufferSize = 1;

    public Terminal(int id, int base, int terminalNumber) {
            super(2);
            this.id = id;
            this.baseAddr = base;
            this.terminalNumber = terminalNumber;
            registers = new int[3];
            readThread = new Terminal.ReadThread();
            readThread.enable(true);
            readThread.setBufferSize(Config.TermBufferSize);
            this.outputBufferSize = Config.TermBufferSize;
            readThread.start();
    }
    
    @Override
    void stop() {
        readThread.enable(false);
        
        
        //readThread.interrupt();
    }

    @Override
    void attachInterconnect(Interconnect connection) {
        this.bus = connection;
    }

    
    @Override
    void advanceTime() {
        //when READ_NEXT_DATA == true
        //remove current dataIn register content, 
        //input not empty --> false
        //System.out.print(".");
        
       
        assert(bus != null);
        int offset = bus.address-this.baseAddr;
        if(!bus.ready && bus.en && offset>=0 && offset<=8) {
            if(bus.rwbar) {
                bus.data = this.registers[offset/4];
            } else {
                int temp = bus.data;
                boolean b0 = this.getControlRegBit(MSK_OUT_BUFFER_FULL);
                boolean b1 = this.getControlRegBit(MSK_IN_BUFFER_NOT_EMPTY);
                this.registers[offset/4] = temp;
                if(offset/4 == controlReg) {
                    if(b0) this.setControlRegBit(MSK_OUT_BUFFER_FULL);
                    else   this.resetControlRegBit(MSK_OUT_BUFFER_FULL);
                    
                    if(b1) this.setControlRegBit(MSK_IN_BUFFER_NOT_EMPTY);
                    else   this.resetControlRegBit(MSK_IN_BUFFER_NOT_EMPTY);
                    execute();
                }
            }
            bus.ready = true;
        }
        //if there is data from input and InRegisterNotEmpty == false
        //move data to InRegister
        outputDataFromBuffer();
        inputDataFromTerminal();
    }
    
    void setOutputBufferSize(int size) {
        this.outputBufferSize = size;
    }
    
    int getOutputBufferSize() {
        return this.outputBufferSize;
    }
    
    void setInputBufferSize(int size) {
        this.readThread.setBufferSize(size);
    }
    
    int getInputBufferSize(){
        return this.readThread.getBufferSize();
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

    
    
    private int inputDelay=0;
    private void inputDataFromTerminal() {
        if(!this.getControlRegBit(this.MSK_IN_BUFFER_NOT_EMPTY)) {
            if(!readThread.isEmpty()) {
                if(inputDelay == 0) {
                    int data = readThread.readBuffer();
                    this.registers[inDataReg] = data;
                    this.setControlRegBit(MSK_IN_BUFFER_NOT_EMPTY);
                    inputDelay = Config.TermDelayAvg;
                } else {
                    if(Config.TermShowInDelay) System.out.print(".");
                    inputDelay -= 1;
                }
                
                
            } else {
                inputDelay = Config.TermDelayAvg;
            }
        }
        if(this.getControlRegBit(this.MSK_IN_BUFFER_NOT_EMPTY)) {
            this.bus.isInterrupted = true;
        }
    }

   
    
    private static class OutputData {
        public int Data, Format;
        
        public OutputData(int data, int format) {
            this.Data = data;
            this.Format = format;
        }
    }
    
    public ArrayList<OutputData> outputBuffer = new ArrayList<OutputData>();
    
   
    
    private void storeOutDataToBuffer() {
        if(!this.getControlRegBit(this.MSK_OUT_BUFFER_FULL)) {
            OutputData data = new OutputData(this.registers[outDataReg], outputFormat());
            outputBuffer.add(data);
            resetControlRegBit(MSK_WRITE_DATA);
            
            if(outputBuffer.size() == this.outputBufferSize) {
                this.setControlRegBit(this.MSK_OUT_BUFFER_FULL);
            }
        }
    }
    
    private int outputDelay=0;
    private void setOutputDelay() {
        Random rand = new Random();
        if(Config.TermDelayAvg > 10){
            int var = rand.nextInt(Config.TermDelayAvg / 10 +1);
            var = var-Config.TermDelayAvg / 20;

            outputDelay = Config.TermDelayAvg+var;
            if(outputDelay < 0) outputDelay = 0;
        } else {
            outputDelay = Config.TermDelayAvg;
        }
                

    }
    private void outputDataFromBuffer() {
        if(outputBuffer.size() != 0) {
            if(outputDelay == 0) {
                outputDataToTerminal(outputBuffer.get(0));
                outputBuffer.remove(0);
                this.resetControlRegBit(MSK_OUT_BUFFER_FULL);
                setOutputDelay();
            } else {
                outputDelay -= 1;
                if(Config.TermShowOutDelay) System.out.print("-");
            }
            
        } else {
            setOutputDelay();
        }
        //output buffer no longer full
        
    }
    
    
    
    private void outputDataToTerminal(OutputData data) {
        if(FORMAT_INT == data.Format) {
            System.out.print(data.Data);
        } else {
            System.out.print((char)data.Data);
        }
    }

    private void prepareReadNextData() {
        //clear READ_NEXT_DATA so it won't redo this function again
        resetControlRegBit(MSK_READ_NEXT_DATA);
        resetControlRegBit(MSK_IN_BUFFER_NOT_EMPTY);
        
    }
    
    private void execute(){
       
        if(requireWriteData()) {
            storeOutDataToBuffer();
        
            //return;
        }
        if(getControlRegBit(MSK_READ_NEXT_DATA)) {
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