   M S B A M L   `   `   `  ����F  BBusyManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:BusyManagerBusyManager  5   	   #    BusyManager.ChangeStateDialog   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:BusyManager     5   	     � Title$  Change ��5   	   $	��215��6   $	��193��6&   $	��193��62   $	��215��6A    � WindowStartupLocation$ CenterOwner=�5	   	    � WindowStyle$ 
ToolWindow=�6-    � ShowInTaskbar .  $��Center=�6\   $��Center=�6y    � 
ResizeMode$ NoResize=�6�   .��5
      � $��0,0,4,0q�6   .+�5   
   �� -   5         ��Name$ label��5      $��Begin��6   $
��Left=�6/   $��10,8,0,0q�6J   $	��Top=�6\   (  "System.Windows.Controls.DatePicker  5   
   -   5          Name$ BeginPicker��5      $
��Left=�6*   $��	55,10,0,0q�6E   $	��Top=�6X   $	��115��6�     5   
   -   5        $ 	EndPicker��5      $
��Left=�6(   $��	55,67,0,0q�6C   $	��Top=�6V   $	��115��6�   �� 5   
   -   5        $ label1��5      $	��End��6    $
��Left=�6.   $��	10,66,0,0q�6I   $	��Top=�6\   �� 5   
   -   5         ��Name$ ConfirmButton��5      $��Confirm��6(   $��Center=�6:   $��	0,151,0,0q�6W   $	��Top=�6j   $	��102��6�   6�   �� 5   
   -   5         ��Name$ DaysBox��5      $
��Left=�6#   $��23��6>   $��	55,39,0,0q�6J   	 �� TextWrapping$
	 Wrap=�6]   $��1��6q   $	��Top=�6z   $	��115��6�   6�   �� 5   
   -   5        $ label2��5      $
��Days��6    $
��Left=�6/   $��	10,35,0,0q�6J   $	��Top=�6]   �� 5   
   -   5        
 ��Name$
 StateChoiseBox��5      $
��Left=�6+   $��	55,96,0,0q�6F   $	��Top=�6Y   $	��115��6q    �� SelectedIndex$ 1��6}   .@�5      �� $
��Free��6   �� 5      $
��Busy��6   �� 5      $��	Available��6   �� 5      $��notAvailable��6   �� 5      $��Maintenance��6   5      �� 5   
   -	   5        $ label3��5      $��State��6    $
��Left=�60   $��	10,92,0,0q�6K   $	��Top=�6^   �� 5   
   -
   5        $ CustomerBox��5      $
��Left=�6'   $��23��6B   $��
10,123,0,0q�6N   $
	 Wrap=�6b   $��Customer��6v   $	��Top=�6�   $	��160��6�   5      5      