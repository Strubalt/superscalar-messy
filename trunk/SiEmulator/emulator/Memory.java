package emulator;

public class Memory extends Component {

	int size;
	int base_address;
	int[] data;

	
	public Memory(int size, int base_address, int[] initial_data) {
		super(1);
		// TODO Auto-generated constructor stub
		this.size = size;
		this.base_address = base_address;
		this.data = new int[size];
		for(int i=0; i<initial_data.length; ++i) {
			this.data[i] = initial_data[i];
		}
	}

	@Override
	void advanceTime() {
		// TODO Auto-generated method stub

	}

	@Override
	void attachInterconnect(Interconnect connection) {
		// TODO Auto-generated method stub

	}
	
	int read(int address) {
		int inAddr = address - this.base_address;
		int idx = (inAddr-(inAddr % 4))/4;
		switch(address % 4){
		case 0:
			return this.data[idx];
		case 1:
			return ((this.data[idx] >> 8) & 0x00FFFFFF) | ((this.data[idx+1] & 0xFF) << 24);
		case 2:
			return ((this.data[idx] >> 16) & 0x0000FFFF) | ((this.data[idx+1] & 0xFFFF) << 16);
		case 3:
			return ((this.data[idx] >> 24) & 0x000000FF) | ((this.data[idx+1] & 0xFFFFFF) << 8);
		default:
				return 0;
		}
	}
	
	void write(int address, int data) {
		int inAddr = address - this.base_address;
		int idx = (inAddr-(inAddr % 4))/4;
		
		switch(address % 4){
		case 0:
			this.data[idx] = data;
			break;
		case 1:
			this.data[idx] = (this.data[idx] & 0xFF) | (data << 8);
			this.data[idx+1] = (this.data[idx+1] & 0xFFFFFF00) | ((data >> 24) & 0x000000FF);
			break;
		case 2:
			this.data[idx] = (this.data[idx] & 0xFFFF) | (data << 16);
			this.data[idx+1] = (this.data[idx+1] & 0xFFFF0000) | ((data >> 16) & 0x0000FFFF);
			break;
		case 3:
			this.data[idx] = (this.data[idx] & 0xFFFFFF) | (data << 24);
			this.data[idx+1] = (this.data[idx+1] & 0xFF000000) | ((data >> 8) & 0x00FFFFFF);
			break;
		
		}
		
	}
	
	
}
