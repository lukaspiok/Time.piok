   M S B A M L   `   `   `  ����   	Time.piok    Time.piok.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  	Time.piok��$	��350��$	��720�� � 
ResizeMode$ CanResizeWithGrip=�.��� .+��� -   $��listview��$ �� IsSynchronizedWithCurrentItem$
 TrueY�$��0,21,0.4,-0.2q� �� View �� .P��� $��Startnr.�� �� Width$ 50�� �� DisplayMemberBinding ��  �� Path$ Startnummer��� $
��Rang��$ 50�� �� $
 Rang��� $��Vorname��$	 110�� �� $ Vorname��� $��Nachname��$	 140�� �� $ Nachname��� $��Klasse��$	 130�� �� $ Klasse��� $��Status��$ 40�� �� $ Status��� $
��Zeit��$	 100�� �� $ Endzeit� �� StringFormat$ {0:mm}:{0:ss}.{0:ff}�� �� FallbackValue$ 00:00.00���� $��Abstand��$ 60�� �� $ Abstand�	 �� 
GroupStyle	 (  "System.Windows.Controls.GroupStyle  
   HeaderTemplate
 �� .R��� $
��Bold"�$��14(����� *Name+o� -   $
��Menu��$
��Left=�$��22��$��1,-1,0,0q�$	��Top=�$	��719��.@�m� $��Teilnehmer verwalten��.@�m� -   $��btn_add_comp��$��Teilnehmer hinzufügen��m� -   $��btn_remove_comp��$��Teilnehmer entfernen��m� -   $��btn_comp_excel��$��Aus Exceldatei auslesen��m� -   $��btn_add_class��$��Klassen verwalten��m� -   $��btn_dsq��$	��DSQ��m� -	   $��btn_settings��$��Einstellungen��m� -
   $��btn_starttiming��$��Zeitnehmung starten��m� -   $��	btn_print��$��Drucken��m� -    m�Name$ 	btn_close��$��
Schliessen��