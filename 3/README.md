## Internet Programming homework 1
## Require
<ul>
    <li>OS : Ubuntu 16.04 LTS</li>      
    <li>Compiler : gcc version 5.4.0</li>     
</ul>

## How it to work
1. gcc firewall.c -o firewall -lpcap  
***(You NEED to creat a blank "output.pcap" before step 2)***  
2. ./firewall rule.txt trace.pcap output.pcap 
3. tcpdump -r output.pcap  
  
## Rule file format
*\<Source Address\>* *\<Destination Address\>* *\<Source Port\>* *\<Destination Port\>*
  
  You also can use keyword ***"any"*** to represent every address or port.

**EXAMPLE**  
Drop packet from 10.0.2.15:41750 to any destination address port 443  
> 10.0.2.15 any 41750 443  

Drop any packet to 53  
> any any any 53