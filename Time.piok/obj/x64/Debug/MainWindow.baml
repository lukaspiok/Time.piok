   M S B A M L   `   `   `  ����  	Time.piok    Time.piok.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  	Time.piok��5   	   $	��350��6   $	��720��6(    � 
ResizeMode$ CanResizeWithGrip=�64   .��5      � .+�5   
   �� -   5        $��listview��5      $ �� IsSynchronizedWithCurrentItem$
 TrueY�6#   $��0,21,0.4,-0.2q�6H    �� View 5      �� 5      .P�5	      �� $��Startnr.��6%    �� Width$ 50��67    �� DisplayMemberBinding 6B   ��  �� Path$ Startnummer��� 5
      $
��Rang��6%   $ 50��63    6>   �� $
 Rang��� 5      $��Vorname��6%   $	 110��66    6B   �� $ Vorname��� 5      $��Nachname��6%   $	 140��67    6C   �� $ Nachname��� 5      $��Klasse��6%   $	 130��65    6A   �� $ Klasse��� 5      $��Status��6%   $ 40��65    6@   �� $ Status��� 5      $
��Zeit��6%   $	 100��63    6?   �� $ Endzeit� �� StringFormat$ {0:mm}:{0:ss}.{0:ff}�� �� FallbackValue$ 00:00.00���� 5      $��Abstand��6%   $ 60��66    6A   �� $ Abstand�5      	 �� 
GroupStyle	 5      (  "System.Windows.Controls.GroupStyle  5      
   HeaderTemplate
 5      �� 5      .R�5      �� $
��Bold"�6(   $��14(�6:   ��6H   �� *Name+5      5      5      o� 5   
   -   5        $
��Menu��5      $
��Left=�6   $��22��66   $��1,-1,0,0q�6B   $	��Top=�6T   $	��719��6l   .@�5      m� $��Teilnehmer verwalten��6   .@�5      m� -   5        $��btn_add_comp��5      $��Teilnehmer hinzufügen��6+   6J   m� 5       -   5        $��btn_remove_comp��5       $��Teilnehmer entfernen��62   6P   m� 5!      -   5        $��btn_comp_excel��5!      $��Aus Exceldatei auslesen��61   6S   5"      m� 5#      -   5        $��btn_add_class��5#      $��Klassen verwalten��6,   6G   m� 5$      -   5        $��btn_dsq��5$      $	��DSQ��6&   63   m� 5%      -	   5        $��btn_settings��5%      $��Einstellungen��6+   6B   m� 5&      -
   5        $��btn_starttiming��5&      $��Zeitnehmung starten��6.   6K   m� 5'      -   5        $��	btn_print��5'      $��Drucken��6(   m� 5(      -   5         m�Name$ 	btn_close��5(      $��
Schliessen��6*   6>   5)      5*      5+      