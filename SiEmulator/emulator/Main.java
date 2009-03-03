package emulator;
import java.util.*;
import java.io.*;

public class Main {

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		// TODO Auto-generated method stub
		
		String inFile = null;
		
		if(args.length != 1) {
			System.out.println("Please enter .out filename.");
			return;
			//Asm.assemble(inFile, outFile);
		} else {
			inFile = args[0];
		}
		Memory mem = new Memory(1 << 20, 0, readInputFile(inFile));
		
		System.out.print(1 << 10);
	}
	
	private static int[] readInputFile(String inFile) {
		java.io.File f = new java.io.File(inFile);
		if(!f.isFile()) return null;
		ArrayList<Integer> al = new ArrayList<Integer>();
		String current;
		try {
			BufferedReader br = new BufferedReader(new FileReader(inFile));
			while ((current = br.readLine()) != null) { // while loop begins here
				//System.out.println(current);
				String data = current.split(" ")[1];
				int upper = Integer.parseInt(data.substring(0, 16), 2);
				int lower = Integer.parseInt(data.substring(16, 32), 2);
				
				al.add(new Integer((upper << 14) | lower));
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
