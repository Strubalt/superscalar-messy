package emulator;
import java.util.*;


public class Processor extends Component {
	
	private ArrayList<Interconnect> interconnects = new ArrayList<Interconnect>();
	private int registers[];
	private int stage; //IF=0, DEC=1, EX, MEM, WB
	private int ring;
	private int pc;
	

	public Processor() {
		super(0);
		pc = 0;
		ring = 0;
		stage = 0;
		registers = new int[64];
	}

	@Override
	void attachInterconnect(Interconnect connection) {
		// TODO Auto-generated method stub
		this.interconnects.add(connection);
		
	}

	@Override
	void advanceTime() {
		// TODO Auto-generated method stub

	}
	
	int getReg(int registerNumber) {
		return this.registers[registerNumber];
	}
	
	void setReg(int registerNumber, int value) {
		this.registers[registerNumber] = value;
	}
	
	int getRing() {
		return this.ring;
	}
	
	void setRing(int ringLevel) {
		this.ring = ringLevel;
	}
	
	void setPC(int address) {
		this.pc = address;
	}
	
	void executeOpcode(int instruction) {
		int op = getOp(instruction);
		
	}
	
	void dumpRegs() {
		Long l = new java.lang.Long(1);
		
		for(int i=0; i<this.registers.length; ++i) {
			System.out.println("R" + new Integer(i).toString() + 
							" = " + new Integer(this.registers[i]).toString());
		}
	}
	
	private int getOp(int instruction) {
		return ((instruction >> 26) & 0x3F);
	}

}
