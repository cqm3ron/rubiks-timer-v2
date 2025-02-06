Imports System
Imports System.IO
Imports System.Threading

Module Program
    Dim entriesFolder As String = "P:/RubiksCompetition/Entries/"
    Dim timer As New Stopwatch
    Dim timerRunning As Boolean
    Sub Main(args As String())
        Startup()
    End Sub
    Sub DisplayHeading()
        Dim colours As New List(Of Integer)
        Dim title As String = "RUBIKS TIMER V2"
        Dim randomNumber As Integer
        Dim usedColours As New List(Of String)
        Dim bufferSize As Integer = 20
        For i = 0 To title.Length - 1
            If usedColours.Count = 11 Then
                usedColours.Clear()
            End If
            Do
                randomNumber = (Int(Rnd() * 14) + 1)
                If Not (randomNumber = 7 Or randomNumber = 8) Then
                    If Not usedColours.Contains(randomNumber) Then
                        Exit Do
                    End If
                End If
            Loop
            colours.Add(randomNumber)
            usedColours.Add(randomNumber)
        Next

        Console.SetCursorPosition(0, 0)
        For i = 0 To (title.Length - 1) + (2 * bufferSize)
            Console.Write("=")
        Next
        Console.WriteLine()
        For i = 0 To bufferSize
            Console.Write(" ")
        Next
        For i = 0 To title.Length - 1
            Console.ForegroundColor = colours(i)
            Console.Write(title(i))
            Console.ResetColor()
        Next
        For i = 0 To bufferSize
            Console.Write(" ")
        Next
        Console.WriteLine()
        For i = 0 To (title.Length - 1) + (2 * bufferSize)
            Console.Write("=")
        Next
        Console.WriteLine()
    End Sub
    Function GetName(firstName As Boolean)
        Dim name As String
        Dim success As Boolean = False
        Do
            Console.Clear()
            DisplayHeading()
            Console.WriteLine()
            Console.Write("Please enter your")
            Console.ForegroundColor = ConsoleColor.Magenta
            If firstName Then
                Console.Write(" first name")
            Else
                Console.Write(" last name")
            End If
            Console.ResetColor()
            Console.Write(": ")
            Try
                name = Console.ReadLine()
                If name.Contains(" "c) Then
                    If name.EndsWith(" "c) Then
                        Do
                            name = RemoveEndingSpaces(name)
                        Loop Until Not name.EndsWith(" "c)
                        If name.Contains(" "c) Then
                            If firstName Then
                                Throw New Exception("Please enter only your first name")
                            Else
                                Throw New Exception("Please enter only your last name")
                            End If
                        End If
                    Else
                        If firstName Then
                            Throw New Exception("Please enter only your first name")
                        Else
                            Throw New Exception("Please enter only your last name")
                        End If
                    End If
                End If
                name = CapitaliseFirstLetter(name)
                success = True
            Catch ex As Exception
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine($"Error: {ex.Message}")
                Console.ResetColor()
                Console.WriteLine("Please try again")
                Thread.Sleep(1500)
            End Try
        Loop While success = False
        Return name
    End Function
    Function CapitaliseFirstLetter(word As String)
        Dim wordLetterList As List(Of Char) = word.ToCharArray.ToList
        Dim firstLetter As String = wordLetterList(0).ToString.ToUpper
        wordLetterList(0) = firstLetter(0)
        Return word
    End Function
    Function RemoveEndingSpaces(word As String)
        Dim wordLetterList As List(Of Char) = word.ToCharArray.ToList
        wordLetterList.RemoveAt(word.Length - 1)
        word = ""
        For Each letter In wordLetterList
            word += letter
        Next
        Return word
    End Function
    Sub OptionsDisplay(currentPos As (Integer, Integer), Optional Selected As Integer = 0)
        Dim yValue As Integer = currentPos.Item2 + 2
        Console.ResetColor()
        Console.SetCursorPosition(currentPos.Item1, yValue)
        Console.WriteLine("+-----------------------------------------------------+")
        Console.Write("|                     ")
        If Selected = 0 Then
            Console.Write(">")
            Console.ForegroundColor = ConsoleColor.Blue
        Else
            Console.Write(" ")
            Console.ResetColor()
        End If
        Console.Write(" NEW ENTRY ")
        Console.ResetColor()
        If Selected = 0 Then
            Console.Write("<")
        Else
            Console.Write(" ")
            Console.ResetColor()
        End If

        Console.WriteLine("                   |")
        Console.WriteLine("+-----------------------------------------------------+")
        Console.Write("|                    ")

        If Selected = 1 Then
            Console.Write(">")
            Console.ForegroundColor = ConsoleColor.Blue
        Else
            Console.ResetColor()
            Console.Write(" ")
        End If
        Console.Write(" LEADERBOARD ")
        Console.ResetColor()
        If Selected = 1 Then
            Console.Write("<")
            Console.ForegroundColor = ConsoleColor.Blue
        Else
            Console.ResetColor()
            Console.Write(" ")
        End If
        Console.ResetColor()
        Console.WriteLine("                  |")
        Console.WriteLine("+-----------------------------------------------------+")
    End Sub
    Sub Startup()
        Console.Clear()
        Console.CursorVisible = False
        Dim currentPos As (Integer, Integer) = Console.GetCursorPosition()
        Dim selected As Integer = 0
        Console.WriteLine("Would you like to view the leaderboard or make a new entry?")
        Console.WriteLine("Use the arrow keys to choose your option and press enter to select:")
        Dim keyPressed As ConsoleKeyInfo
        OptionsDisplay(currentPos)
        Do
            If Console.KeyAvailable Then
                keyPressed = Console.ReadKey(True)
            End If
            If keyPressed.Key = ConsoleKey.DownArrow Then
                OptionsDisplay(currentPos, 1)
                selected = 1
            ElseIf keyPressed.Key = ConsoleKey.UpArrow Then
                OptionsDisplay(currentPos, 0)
                selected = 0
            End If
            If keyPressed.Key <> ConsoleKey.Enter Then
                keyPressed = Nothing
            End If
        Loop Until keyPressed.Key = ConsoleKey.Enter

        If selected = 0 Then
            Entry()
        Else
            Leaderboard()
        End If


    End Sub
    Sub Leaderboard()
        Console.Clear()
        DisplayHeading()
        Console.WriteLine()
        Console.WriteLine("Leaderboard:")
        Console.WriteLine("------------")
        If Directory.Exists(entriesFolder) Then
            Dim files = Directory.GetFiles(entriesFolder, "*.rubiksv2")
            Dim leaderboard As New List(Of (String, TimeSpan))

            For Each file In files
                Using sr As New StreamReader(file)
                    Dim line As String = sr.ReadLine()
                    If Not String.IsNullOrEmpty(line) Then
                        Dim parts = line.Split("-"c)
                        If parts.Length = 2 Then
                            Dim name = parts(0).Trim()
                            Dim timeParts = parts(1).Trim().Split(":"c)
                            If timeParts.Length = 3 Then
                                Dim minutes = Integer.Parse(timeParts(0))
                                Dim seconds = Integer.Parse(timeParts(1))
                                Dim milliseconds = Integer.Parse(timeParts(2))
                                Dim time = New TimeSpan(0, 0, minutes, seconds, milliseconds)
                                leaderboard.Add((name, time))
                            End If
                        End If
                    End If
                End Using
            Next

            leaderboard = leaderboard.OrderBy(Function(entry) entry.Item2).ToList()

            For value = 0 To leaderboard.Count - 1
                If value = 0 Then
                    Console.ForegroundColor = ConsoleColor.DarkYellow
                ElseIf value = 1 Then
                    Console.ForegroundColor = ConsoleColor.DarkGray
                ElseIf value = 2 Then
                    Console.ForegroundColor = ConsoleColor.Red
                Else
                    Console.ResetColor()
                End If
                Console.Write($"{leaderboard(value).Item1} - {leaderboard(value).Item2.Minutes}m {leaderboard(value).Item2.Seconds}s {leaderboard(value).Item2.Milliseconds}ms")
                If value = 0 Then
                    Console.ForegroundColor = ConsoleColor.Magenta
                    Console.WriteLine(" - HIGHEST SCORE!")
                Else
                    Console.WriteLine()
                End If
                Console.ResetColor()
            Next
        Else
            Console.WriteLine("No entries found.")
        End If

        Console.WriteLine()
        Console.WriteLine("Press any key to return to the main menu.")
        Console.ReadKey(True)
        Startup()
    End Sub
    Sub Entry()
        Console.Clear()
        Console.CursorVisible = True
        Randomize()
        Dim fileAddition, firstName, lastName As String
        firstName = GetName(True)
        lastName = GetName(False)

        fileAddition = entriesFolder + $"{DateTime.Now:dd-MM-yyyy_HH-mm-ss}_{firstName} {lastName}.rubiksv2"
        Directory.CreateDirectory(entriesFolder)
        Using fs As FileStream = File.Create(fileAddition)
        End Using

        Console.Clear()
        Dim timeTaken As TimeSpan = StartAndStopTimer()

        Using sw As StreamWriter = File.AppendText(fileAddition)
            sw.WriteLine($"{firstName} {lastName} - {timeTaken.Minutes}:{timeTaken.Seconds}:{timeTaken.Milliseconds:000}")
        End Using
        File.SetAttributes(fileAddition, FileAttributes.ReadOnly)

        Console.WriteLine($"Timer stopped at: {timeTaken.Minutes}m {timeTaken.Seconds}s {timeTaken.Milliseconds:000}ms")
        Console.WriteLine("Your time has been recorded and can now be viewed in the leaderboard")
        Console.WriteLine()
        Console.WriteLine("Press any key to close the application.")
        Console.ReadKey(True)
        Environment.Exit(0)
    End Sub
    Function StartAndStopTimer() As TimeSpan
        Console.CursorVisible = False
        Dim elapsedTime As TimeSpan = Nothing
        Dim timerThread As Thread = Nothing
        If timerThread Is Nothing OrElse Not timerThread.IsAlive Then
            timerThread = New Thread(
                Sub()
                    Do
                        SyncLock timer
                            Console.SetCursorPosition(0, 4)
                            Console.Write("                    ")
                            Console.SetCursorPosition(0, 4)
                            Console.Write($"Elapsed: {timer.Elapsed.Minutes}:{timer.Elapsed.Seconds}:{timer.Elapsed.Milliseconds:000}")
                        End SyncLock
                        Thread.Sleep(100)
                    Loop Until Not timerRunning
                End Sub
            )
        End If

        Console.Clear()
        Console.WriteLine("Press any key to start the timer")
        Console.WriteLine("+-----------------------------------------------------+")
        Console.Write("|                    ")
        Console.Write(">")
        Console.ForegroundColor = ConsoleColor.Blue
        Console.Write(" START TIMER ")
        Console.ResetColor()
        Console.Write("<")
        Console.WriteLine("                  |")
        Console.WriteLine("+-----------------------------------------------------+")
        Console.ReadKey(True)

        timer.Reset()
        timer.Start()
        timerRunning = True
        If Not timerThread.IsAlive Then
            timerThread.Start()
        End If

        Console.Clear()
        Console.WriteLine("Press any key to stop the timer")
        Console.WriteLine("+-----------------------------------------------------+")
        Console.Write("|                     ")
        Console.Write(">")
        Console.ForegroundColor = ConsoleColor.Blue
        Console.Write(" END TIMER ")
        Console.ResetColor()
        Console.Write("<")
        Console.WriteLine("                   |")
        Console.WriteLine("+-----------------------------------------------------+")

        Console.ReadKey(True)

        timerRunning = False
        timer.Stop()
        elapsedTime = timer.Elapsed

        If timerThread.IsAlive Then
            timerThread.Join()
        End If

        Console.Clear()
        timer.Reset()

        Return elapsedTime
    End Function

End Module
