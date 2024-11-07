using System;
using System.IO;



public class TextEditor : StatusCs
{
    public string? CabFamilyType { get; set; } // Nokia, Huawei.
    public string? CabType { get; set; } //MA5818, MA 5600, MSAN500, GPON-300, GPON_T500, MODEL C, MODEL B.
    public string? CabCode_1 { get; set; }
    public string? CabCode_2 { get; set; }


    public string? TED_Mg_Gateway_IP { get; set; } //1.1.1.0
    public string? TED_Mg_SH1_IP { get; set; } //1.1.1.1
    public string? TED_Mg_SH2_IP { get; set; } //1.1.1.1


    public string? Mg_Gateway_IP { get; set; } //2.2.2.1
    public string? Mg_SH1_IP { get; set; } //2.2.2.1
    public string? Mg_SH2_IP { get; set; } //2.2.2.1
    public string? Mg_SH3_IP { get; set; }

    public string? Sig_Gateway_IP { get; set; } //3.3.3.2
    public string? Sig_SH1_IP { get; set; } //3.3.3.2
    public string? Sig_SH2_IP { get; set; } //3.3.3.2

    public string? FVNO_EM_Gateway_IP { get; set; } // 4.4.4.3
    public string? FVNO_EM_SH1_IP { get; set; } // 4.4.4.3
    public string? FVNO_EM_SH2_IP { get; set; } // 4.4.4.3

    public string? POP_Name { get; set; }

    private int? processCount { get; set; }
    private string? ReferenceFilePath { get; set; }
    private string? newPath { get; set; }
    private string? FolderName { get; set; }
    private string? Folder_Path { get; set; }
    private string? File_Path { get; set; }
    private int processCounter = 0;


    public TextEditor()
    { }
    public TextEditor(string? cabfamilytype, string? cabtype, string? cabcode_1 = null, string? cabcode_2 = null)  // test result {0-Done}
    {
        CabFamilyType = cabfamilytype;
        CabType = cabtype;
        CabCode_1 = cabcode_1;
        CabCode_2 = cabcode_2;
        // set the process counter according to the type.
        if (cabfamilytype == "Huawei" && cabtype == "MA5818")
            processCount = 4;
        else if (cabfamilytype == "Nokia" && cabtype == "MODEL_B")
            processCount = 2;
        else
            processCount = 1;

        Console.WriteLine("The process count is: " + processCount); // test code.
    }

    public STATUS Getinfo(string? cabcode) // Test Result {4- Done}
    {
        try
        {
            // Excel Function that will return all the data from the Axel sheets.
            Excel.GetIPs(this, cabcode);
            return STATUS.Success;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return STATUS.Failed;
        }
    }

    public STATUS CreateCommission()                    // Test Result {3- Done}
    {
        try
        {
            // select the process route according to the family type.
            switch (CabFamilyType)
            {
                case "Nokia":
                    {
                        do
                        {
                            Getinfo(CabCode_1);
                        } while (processCounter < processCount);
                        break;
                    }
                case "Huawei":
                    {
                        do
                        {
                            if (processCounter == 0)
                            {
                                Getinfo(CabCode_1); // Get all needed Data.
                                GetReference(); // Get the reference file.
                            }
                            else if (processCounter == 1)
                                GetReference(); // Get the reference file.
                            else if (processCounter == 2)
                                Getinfo(CabCode_2); // Change the cabinet code and get the new IPs. 

                            SetFolder();

                            if ((processCounter == 0) || (processCounter == 2))
                            {
                                CreateHuaweiCommission(TED_Mg_SH1_IP, TED_Mg_Gateway_IP, Mg_SH1_IP, Mg_Gateway_IP, Sig_SH1_IP, Sig_Gateway_IP, FVNO_EM_SH1_IP, FVNO_EM_Gateway_IP);
                            }
                            else
                            {
                                CreateHuaweiCommission(TED_Mg_SH2_IP, TED_Mg_Gateway_IP, Mg_SH2_IP, Mg_Gateway_IP, Sig_SH2_IP, Sig_Gateway_IP, FVNO_EM_SH2_IP, FVNO_EM_Gateway_IP);
                            }
                            Console.WriteLine("The process counter is: " + processCounter); // test code.
                            processCounter++;
                        } while (processCounter < processCount);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return STATUS.Success;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return STATUS.Failed;
        }
    }


    private STATUS CreateHuaweiCommission(string? IP_1_1, string? IP_1_0, string? IP_2_2, string? IP_2_1, string? IP_3_3, string? IP_3_2, string? IP_4_4, string? IP_4_3)    // Test Result {5-}
    {
        try
        {
            Dictionary<string, string?> File_pairs = new Dictionary<string, string?>()
            {
                {"1.1.1.1", IP_1_1},
                {"1.1.1.0", IP_1_0},
                {"2.2.2.2", IP_2_2},
                {"2.2.2.1", IP_2_1},
                {"3.3.3.3", IP_3_3},
                {"3.3.3.2", IP_3_2},
                {"4.4.4.4", IP_4_4},
                {"4.4.4.3", IP_4_3},
                {"(11-2-2-2)", ""}
            };
            if (ReferenceFilePath != null && newPath != null)
            {
                using (FileStream readStream = new FileStream(ReferenceFilePath, FileMode.Open, FileAccess.Read))
                using (StreamReader reader = new StreamReader(readStream))
                {
                    // Use FileStream and StreamWriter to write to a new file
                    using (FileStream writeStream = new FileStream(newPath, FileMode.Create, FileAccess.Write))
                    using (StreamWriter writer = new StreamWriter(writeStream))
                    {
                        string? line;
                        // Read the file line by line
                        while ((line = reader.ReadLine()) != null)
                        {
                            foreach (var pair in File_pairs)
                            {
                                if (pair.Key == "(11-2-2-2)")
                                {
                                    switch (processCounter)
                                    {
                                        case 0:
                                            line = line.Replace("(00-00-00-00)", $"({CabCode_1})");
                                            break;
                                        case 1:
                                            line = line.Replace("(00-00-00-00)(SH2)", $"({CabCode_1})(SH2)");
                                            break;
                                        case 2:
                                            line = line.Replace("(00-00-00-00)(SH2)", $"({CabCode_2})(SH1)");
                                            break;
                                        case 3:
                                            line = line.Replace("(00-00-00-00)(SH2)", $"({CabCode_2})(SH2)");
                                            break;
                                        default:

                                            break;
                                    }
                                }
                                else
                                    line = line.Replace(pair.Key, pair.Value);
                            }
                            // Write the updated line to the temporary file
                            writer.WriteLine(line);
                        }
                    }
                }
                return STATUS.Success;
            }
            else
            {
                return STATUS.Error;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
            return STATUS.Error;
        }
    }

    public STATUS GetReference()                       // Test Result {1- Done}
    {
        try
        {
            // from the type of cabinet, formate the reference file.
            switch (CabType)
            {
                case "MA5818":
                    if (processCounter == 0)
                    {
                        ReferenceFilePath = @"references\Models\MA5818\MA5818-8-2024-SH1.cfg";
                        ReferenceFilePath = Path.GetFullPath(ReferenceFilePath);
                        Console.WriteLine(" ReferenceFilePath = " + ReferenceFilePath);
                    }
                    else
                    {
                        ReferenceFilePath = @"references\Models\MA5818\MA5818-8-2024-SH2-3-4.cfg";
                        ReferenceFilePath = Path.GetFullPath(ReferenceFilePath);
                        Console.WriteLine(" ReferenceFilePath = " + ReferenceFilePath);
                    }
                    break;
                case "MA5600":
                    ReferenceFilePath = @"references\Models\MA_5600\MSAN-500-UPPER-2023.cfg";
                    ReferenceFilePath = Path.GetFullPath(ReferenceFilePath);
                    break;
                case "GPON300":
                    ReferenceFilePath = @"references\Models\GPON_300\GPON-300.cfg";
                    ReferenceFilePath = Path.GetFullPath(ReferenceFilePath);
                    break;
                case "GPON_T500":
                    ReferenceFilePath = @"references\Models\GPON_T500\GPON-T500.cfg";
                    ReferenceFilePath = Path.GetFullPath(ReferenceFilePath);
                    break;
                case "MSAN500":
                    ReferenceFilePath = @"references\Models\MSAN_500\MSAN-500-UPPER-2023.cfg";
                    ReferenceFilePath = Path.GetFullPath(ReferenceFilePath);
                    break;
            }

            // Console.WriteLine(" ReferenceFilePath = " + ReferenceFilePath); // test code.
            return STATUS.Success;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
            return STATUS.Error;
        }
    }

    public STATUS SetFolder()                          // Test result {2- Done}
    {
        try
        {
            Console.WriteLine("processCounter = " + processCounter); // test code.

            // (1) create a new folder with the name of the cabinet code if the counter is zero.
            if (processCounter == 0)
            {
                if (CabCode_2 != null)
                {
                    string[] parts = CabCode_2.Split('-');
                    FolderName = CabCode_1 + " & " + parts[parts.Length - 1]; // Get the last part
                }
                else
                    FolderName = CabCode_1;

                Folder_Path = $@"Outputs\{FolderName}\";
                Folder_Path = Path.GetFullPath(Folder_Path);
                if (!Directory.Exists(Folder_Path))
                    Directory.CreateDirectory(Folder_Path);
            }

            // (2) chang the name that the new file will take.
            if (processCounter < 2)
            {
                if (processCounter == 0)
                    File_Path = CabCode_1 + "-SH1.cfg";
                else
                    File_Path = CabCode_1 + "-SH2.cfg";
            }
            else
            {
                if (processCounter == 2)
                    File_Path = CabCode_2 + "-SH1.cfg";
                else
                    File_Path = CabCode_2 + "-SH2.cfg";
            }

            // (3) set the new path for the new file.
            newPath = Folder_Path + File_Path;

            // (4) return the status of the process.
            return STATUS.Success;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
            return STATUS.Error;
        }
    }
}
