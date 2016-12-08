#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include <netinet/in.h>
#include <netinet/ip.h>
#include <net/if.h>
#include <netinet/if_ether.h>

#include <pcap.h>

void ruleFormatter(FILE *filePtr, char *ruleString);

int main(int argc, char *argv[])
{
    pcap_t *pcap;
    char *device;                   
    char errBuf[PCAP_ERRBUF_SIZE];
    const u_char *packet;
    struct pcap_pkthdr header;      
    struct bpf_program fp;          // For using pcap_compile()
    bpf_u_int32 netMask;            // Subnet mask from pcap_lookupnet()
    bpf_u_int32 netIp;              // IP information from pcap_lookupnet()
    int packetCount = 0;            // Count the packet number
    pcap_dumper_t *dumpFile;        // Output file
    FILE *filePtr;                  // Use to open rule file

    /* Skip over the program name */
    ++argv;
    --argc;

    /* We need 3 arguments */
    if (argc != 3)
    {
        fprintf(stderr, "Program requires 3 arguments: <rule file> <input pcap file> <output pcap file>.\n");
        exit(1);
    }

    /* Open rule file and take context to right format */
    char ruleString[1000] = "not (";           // Store the rule
    filePtr = fopen(argv[0], "r");
    if (!filePtr)
    {
        printf("Error opening rule file\n");
        exit(1);
    }    
    ruleFormatter(filePtr, ruleString);
    
    /* 
    // Test Block //
    // To see whether the rule formatter working good //
    */

     printf("*****\n* %s *\n*****\n", ruleString);
     char a;
     scanf("%c", &a);
     

    /* Get a device */
    device = pcap_lookupdev(errBuf);
    if (device == NULL)
    {
        fprintf(stderr, "%s\n", errBuf);
        exit(1);
    }

    /* Ask pcap for the IP address and mask of the device */
    pcap_lookupnet(device, &netIp, &netMask, errBuf);

    /* Open a input pcap file */
    pcap = pcap_open_offline(argv[1], errBuf);
    if (pcap == NULL)
    {
        fprintf(stderr, "Error reading pcap file: %s\n", errBuf);
        exit(1);
    }

    /* Open dump file */
    dumpFile = pcap_dump_open(pcap, argv[2]);
    if (dumpFile == NULL)
    {
        printf("Error opening output file.\n");
        exit(1);
    }

    /* Calling pcap_compile() to compile the string of rules into a filter program */
    if (pcap_compile(pcap, &fp, ruleString, 0, netIp) == -1)
    {
        fprintf(stderr, "Error calling pcap_compile().\n");
        exit(1);
    }

    /* Setting the filter */
    if (pcap_setfilter(pcap, &fp) == -1)
    {
        fprintf(stderr, "Error setting filter.\n");
        exit(1);
    }

    /* Read packet information from pcap file and dump the accepted packets to output file */
    while ((packet = pcap_next(pcap, &header)) != NULL)
    {
        printf("Get a packet i%d.\n", packetCount);
        packetCount++;
        if (packetCount % 2 == 0)
        {
            pcap_dump((unsigned char *)dumpFile, &header, packet);
        }
    }

    return 0;
}

void ruleFormatter(FILE *filePtr, char *ruleString)
{
    char srcAddress[20], dstAddress[20], srcPort[5], dstPort[5];    
    int orAnd = 1;

    fscanf(filePtr, "%s %s %s %s", srcAddress, dstAddress, srcPort, dstPort);
    do
    {
        if (strcmp(srcAddress, "any") != 0)
        {
            strcat(ruleString, "src host ");
            strcat(ruleString, srcAddress);
            orAnd = 1;
        }
                
        if (strcmp(dstAddress, "any") != 0)
        {
            if (orAnd)
            {
                strcat(ruleString, " and ");
            }            
            strcat(ruleString, "dst host ");
            strcat(ruleString, dstAddress);
            orAnd = 1;
        }
        
        if (strcmp(srcPort, "any") != 0)
        {
            if (orAnd)
            {
                strcat(ruleString, " and ");
            }            
            strcat(ruleString, "src port ");
            strcat(ruleString, srcPort);
            orAnd = 1;
        }

        if (strcmp(dstPort, "any") != 0)
        {
            if (orAnd)
            {
                strcat(ruleString, " and ");
            }            
            strcat(ruleString, "dst port ");
            strcat(ruleString, dstPort);
        }        

        strcat(ruleString, " or "); 
        orAnd = 0;
    }
    while(fscanf(filePtr, "%s %s %s %s", srcAddress, dstAddress, srcPort, dstPort) != EOF);

    ruleString[strlen(ruleString) - 4] = ')';
    ruleString[strlen(ruleString) - 3] = '\0';
}