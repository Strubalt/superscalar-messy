package emulator;

public class Terminal extends Component{

	final int controlReg = 0;
	final int outDataReg = 1;
	final int inDataReg = 2;
        
	private Interconnect bus;
	int id, baseAddr, terminalNumber;

	int registers[];

	public Terminal(int id, int base, int terminalNumber) {
		super(2);
		this.id = id;
		this.baseAddr = base;
		this.terminalNumber = terminalNumber;
		registers = new int[3];
	}

	@Override
	void attachInterconnect(Interconnect connection) {
            this.bus = connection;
	}

	@Override
	void advanceTime() {
            assert(bus != null);
            int offset = bus.address-this.baseAddr;
            if(bus.en && offset>=0 && offset<=8) {
                if(bus.rwbar) {
                    bus.data = this.registers[offset/4];
                } else {
                    this.registers[offset/4] = bus.data;
                    if(offset/4 == controlReg) {
                        execute();
                    }
                }
                bus.en = false;
            }
	}
        final int OUT_BUFFER_FILL = 0;
        final int IN_BUFFER_NOT_EMPTY = 1;
        final int WRITE_DATA = 1 << 16;
        final int READ_NEXT_DATA = 1 << 17;
        final int OUT_FORMAT = 1 << 18;
        final int IN_FORMAT = 1 << 19;
        final int FORMAT_INT = 0;
        final int FORMAT_ASCII = 1;
        
        private int outputFormat() {
            if((this.registers[controlReg] & OUT_FORMAT) != 0) {
                return FORMAT_ASCII;
            }
            return FORMAT_INT;
        }
        
        private int inputFormat() {
            if((this.registers[controlReg] & IN_FORMAT) != 0) {
                return FORMAT_ASCII;
            }
            return FORMAT_INT;
        }
        
        private boolean requireWriteData() {
            
            if((this.registers[controlReg] & WRITE_DATA) != 0) {
                return true;
            }
            return false;
        }
        
        private void resetWriteData() {
            this.registers[controlReg] = this.registers[controlReg] & (~WRITE_DATA);
        }
        
        private void resetReadData() {
            this.registers[controlReg] = this.registers[controlReg] & (~READ_NEXT_DATA);
        }
        
        private boolean requireReadData() {
            
            if((this.registers[controlReg] & READ_NEXT_DATA) != 0) {
                return true;
            }
            return false;
        }
        
        private void execute(){
           
            if(requireWriteData()) {
                if(outputFormat() == FORMAT_INT) {
                    System.out.print(this.registers[inDataReg]);
                } else {
                    System.out.print((char)this.registers[outDataReg]);
                }
                resetWriteData();
                return;
            }
            if(requireReadData()) {
                assert(false);
                resetReadData();
                return;
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