   M S B A M L   `   `   `  ����W  SCefSharp.Wpf, Version=116.0.230.0, Culture=neutral, PublicKeyToken=40c4b6fc221f4138A0clr-namespace:CefSharp.Wpf;assembly=CefSharp.WpfCefSharp.Wpf  5   	    WWWApiSearch   eBayDEParser.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   b ^Telerik.Windows.Data, Version=2023.3.1114.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7f bTelerik.Windows.Controls, Version=2023.3.1114.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7o kTelerik.Windows.Controls.GridView, Version=2023.3.1114.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7r	 nTelerik.Windows.Controls.VirtualGrid, Version=2023.3.1114.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7Etelerik1http://schemas.telerik.com/2008/xaml/presentation    	 5   	   @wpf0clr-namespace:CefSharp.Wpf;assembly=CefSharp.Wpf    	 5   	     � Title$  WWW_API_Parser��5   	   $	��700��6    $	��900��6-   .��5      � $��0,0,2,0q�6   ���	#FFF1EDED � ColumnDefinitions 5   
   �� 5	      $	��100 �6   �� 5
      $��3* �6   �� 5      $��* �6   �� 5      $	��220 �6    � RowDefinitions 5   
   �� 5      $��25 �6   �� 5      $��25 �6   �� 5      $��40 �6   �� 5      $��40 �6   �� 5      $��35 �6   �� 5      $��* �6   �� 5      $��35 �6   .+�5   
   �� -   5        $��textbox1��5   D   $��0��6   $��3��6   $��0��64   {�.  $��Stretch=�6f   $��Stretch=�6�   $��	10,5,10,0q�6�    �� TextWrapping$
 Wrap=�6�   �� 5   
   -   5         ��Name$	 csv��5   �   $��0��6   $��1��6   $��3��62   $��Stretch=�6B   $��Stretch=�6^   $��	10,5,10,0q�6|   $��Plik csv��6�   6�   �� 5   
    �� Orientation$ 
Horizontal=�6   $��1��6.   $��3��6;   $��0��6O   $��Stretch=�6_   $
��Left=�6{   $��	10,0,10,0q�6�   .+�5      �� $#��Ilość zadań równoległych��6   �� 5      -   5        $��taskMax��5      $��1��6%   {�.   �� TextAlignment$ Center=�6@   $��Center=�6W   $
 Wrap=�6y   $��50��6�    �� 	UndoLimit$ 0��6�    � ToolTip$k ePodaj maksymalną ilość stron jaka ma być pobrana dla wybranej podkategorii. 0 - Wszystkie strony.��6�   5      �� 5   
   -   5        $ startGet��5   $   $��Pobieraj��6   $��1��66   $��1��6C   $��3��6W   $��Stretch=�6g   $��Stretch=�6�   $��	10,5,10,0q�6�   6�   �� 5    
   -   5        $��Wyczyść bazę��5       $��2��6(   $��1��66   $��3��6J   $��Stretch=�6Z   $��Stretch=�6v   $��	10,5,10,0q�6�   6�   �� 5!   
   -   5        $��Kopiuj błedy do schowka��5!      $��3��63   $��1��6@   $��3��6T   $��Stretch=�6d   $��Stretch=�6�   $��	10,5,10,0q�6�   6�   �� 5"   
   -   5        $��Eksport Excel��5"      $��4��6)   $��1��66   $��3��6J   $��Stretch=�6Z   $��Stretch=�6v   $��	10,5,10,0q�6�   6�   �� 5$   
   -   5        	 ��Name$	 infoMsg��5$   �   $�� ��6   $��	15,0,15,0q�6   ���	#FFF1EDED$	��Top=�6F   $
��Left=�6^   $��48��6y   $
��Wrap=�6�   $��2��6�   $��2��6�   $��0��6�   �� 5%   
   -	   5        $��Count��5%      $�� ��6   $��2��6(   $��1��65   $��2��6I   $��Stretch=�6Y   $��Stretch=�6u   $��	10,5,10,0q�6�   ���	#FFF1EDED
���   �$��1q�6�   �� 5'   
   $ 
Horizontal=�6   $��2��6.   $��1��6;   $��2��6O   $��Center=�6_   $��Center=�6z   $��10,5,5,5q�6�   .+�5(      �� -
   5        $
 play��5(       $�� ��6   $��21��6.   $��35��6:   $ Start��6X   ��5)      �� 5*      
 �� ImageSource$
 media_play.png��6!    j� Stretch$ Uniform=�6>   5,      �� 5-      -   5        $ pause��5-       $�� ��6   $��21��6/   $��35��6;   $ Pausa��6Z   ��5.      �� 5/      $
 	pause.png��6!   $ Uniform=�69   51      �� 52      -   5        $
 cont��52       $�� ��6   $��21��6.   $��35��6:   $ Kontynuacja��6X   ��53      �� 54      $!
 media_play_pause_resume.png��6!   $ Uniform=�6K   56      57      �� 59   
   $��5��6   $��4��6"   $��1��66   $��0��6G   $��10,0,5,0q�6W   .@�5:      �� $
��Dane��6   .��5;      �  5<      �� 5=      $��* �6+   .+�5?      *  $Telerik.Windows.Controls.RadGridView  -   5        $��gridView��5?   �   ;  5Telerik.Windows.Controls.GridView.GridViewDataControl   ShowGroupPanel .    AutoGenerateColumns .     AutoExpandGroups .    SelectionMode$ Single=�6t      EnableRowVirtualization . '    EnableColumnGroupsVirtualization . $��1��6�   $��1��6�   $��Stretch=�6�   $��Stretch=�6"  $��Stretch=�6G  $��Stretch=�6c     Columns 5@      1  +Telerik.Windows.Controls.GridViewDataColumn  5A      6  0Telerik.Windows.Controls.GridViewBoundColumnBase   Header$	 Url��6Z      DataMemberBinding 69   �� *Id+6Z     5B      $ Tytuł��6\    69   �� *Name+6\     5C      $ Tytuł oryginalny��6_    69   �� *	NameOrg+6_     5D      $ Premiera��6`    69   �� *
Register+6`     5E      $ Reżyser��6`    69   �� *
CreateBy+6`     5F      $ Gatunek��6`    69   �� *
Category+6`     5G      $ 	Produkcja��6_    69   �� *	Country+6_   5I      5J      5K      �� 5L      $
��Info��6    �� 
IsSelected . .��5M      �  5N      �� 5O      $��* �6+   .+�5Q      �� -   5        $��ScrollViewer1��5Q   @   $��1��6#   $��1��60   $
��Auto=�6U   $
��Auto=�6x   ��. .��5R      �� -   5        $��browserPanel��5R   %   $ 
Horizontal=�69   $��Stretch=�6R   $��Stretch=�6n   $��2��6�   .+�5S      �� .+�5T   "   �� -   5        $��listbox��5T   *   $	��400��6e   $	��400��6q   $	��400��6~   ��5U   &   �� 5V   *   .R�5W   .   $� 5X   +   5Z   #   5[      %   CefSharp.Wpf.ChromiumWebBrowser  5\      -   5          Name$ Browser��5\   k      Address$ https://www.google.com��65   $��10,10,30,10q�6V   $y�Visible=�6|   $	��500��6�   $
��Auto��6�   $
��Auto��6�   $	��500��6�   6�   5]      5^      5_      5`      5a      �� 5b   
   $��6��6   $��4��6!   $��1��65   $��Stretch=�6F   $��28��6d   $��10,0,0,0q�6p   $��Stretch=�6�   $��Stretch=�6�   .@�5c      �� $��Aktualnie przetwarzany��6   68   �� 5d      �� 5e      -   5        $	 process��5e   Z    � Cursor$
 Hand��6L   $��9(�6k   $��Stretch=�6x   6�   5f      5g      5h      