package emulator;
import java.util.*;
import java.io.*;
import java.lang.*;

public class Emulator {
    
    

	/**
	 * @param args
	 */
	public static void main(String[] args) {
            // TODO Auto-generated method stub

            String inFile = null;

            //System.out.println(new File(System.getProperty("user.dir")));
            if(args.length != 1) {
                    System.out.println("Please enter .out filename.");
                    return;
            } else {
                    inFile = args[0];
            }
            byte a[] = new byte[100];
           
            try {
                int len = System.in.read();
                 System.out.println((char)len);
                len = System.in.read();
                 System.out.println((char)len);
                
             } catch(Exception e) {
                 
             }
            //System.out.println("enter ");
            //PrimeThread p1 = new PrimeThread(1);
            //PrimeThread p2 = new PrimeThread(2);
            //p1.start();
            //p2.start();
            //System.out.print(Integer.toHexString(0x80000000 >> (32-11-2)));
                
	}
        
        private static void run(String inFile) {
            int initialMem[] = readInputFile(inFile);
            Emulator emu = new Emulator(initialMem);
            while(!emu.isHalt()){
                emu.advanceTime();
            }
            emu.dumpRegs();
        }
        
        private Processor processor;
        private Terminal terminal;
        private Memory memory;
        ArrayList<Component> components = new ArrayList<Component>();
        
        public Emulator(int initialMem[]) {
            int terminalBaseAddr = 0xf00;
            
            this.memory = new Memory(900, 0, initialMem);
            terminal = new Terminal(1, terminalBaseAddr, 1);
            processor = new Processor();

            Interconnect connection = new WireInterconnect(2, 
                    new Component[]{processor, memory, terminal});
            components.add(memory);
            components.add(terminal);
        }
        
        void dumpRegs() {
            System.out.println();
            processor.dumpRegs();
        }
                
        public void advanceTime(){
            advanceTime(this.processor, this.components);
        }
        
        private void advanceTime(Processor processor, ArrayList<Component> components) {
            processor.advanceTime();
            for(Component component: components) {
                component.advanceTime();
            }
        }
        
        public boolean isHalt() { return this.processor.isHalt(); }
        
	
	private static int[] readInputFile(String inFile) {
		java.io.File f = new java.io.File(inFile);
		if(!f.isFile()) return null;
		ArrayList<Integer> al = new ArrayList<Integer>();
		String current;
		try {
			BufferedReader br = new BufferedReader(new FileReader(inFile));
			while ((current = br.readLine()) != null) { // while loop begins here
				
				String data = current.split(" ")[1];
				int upper = Integer.parseInt(data.substring(0, 16), 2);
				int lower = Integer.parseInt(data.substring(16, 32), 2);
				
				al.add(new Integer((upper << 16) | lower));
				//System.out.println("    " + Integer.toBinaryString(al.get(al.size()-1).intValue()));
			} // end while 
		} // end try
		catch (IOException e) {
			System.err.println("Error: " + e);
		}
		
		int data[] = new int[al.size()];
		for(int i=0; i<al.size(); ++i) {
			data[i] = al.get(i).intValue();
		}
		return data;
		
	}
	
	private static void testMemory() {
		int data[] = {0xFFFFFFFF, 0xFFFFFF, 0xFFFFFF};
		int baseAddr = 100;
		Random rand = new Random();
		
		Memory m = new Memory(100000, baseAddr, data);
		
		int randValue;
		for(int addr = baseAddr; addr<=baseAddr+96000; addr++) {
			randValue = rand.nextInt();
			m.write(addr, randValue);
			
			if(randValue != m.read(addr)) {
				System.out.println("Error, Addr=" + new Integer(addr).toString() +
						", data=" + Integer.toHexString(randValue) + 
						", read=" + Integer.toHexString(m.read(addr)));
			}
			
		}
	}

}