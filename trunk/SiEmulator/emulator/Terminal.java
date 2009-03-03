package emulator;

public class Terminal extends Component{

	final int controlReg = 0;
	final int outDataReg = 1;
	final int inDataReg = 2;
	
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
		
		
	}

	@Override
	void advanceTime() {
		
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
