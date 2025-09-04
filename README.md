Bu proje, bir Python sunucusu ile bir C# istemcisi arasında OPC UA (Open Platform Communications Unified Architecture) iletişimi kuran basit bir örnektir. Python sunucusu, bir sıcaklık sensörü simüle ederken, 
C# istemcisi bu değeri okur ve günceller.


1. Proje Hakkında

Bu proje, endüstriyel otomasyon ve IoT (Nesnelerin İnterneti) uygulamalarında yaygın olarak kullanılan OPC UA protokolünün temel işlevlerini gösterir. İki ana bileşenden oluşur:

Python Sunucusu (server.py): opcua kütüphanesini kullanarak bir OPC UA sunucusu oluşturur. Bu sunucu, MyObject adlı bir nesne ve içinde "Temperature" adlı bir değişken barındırır. 
Sunucu, sıcaklık değerini düzenli olarak günceller ve istemcilerin bu değere erişmesine izin verir.

C# İstemcisi (Program.cs): Opc.Ua.Client kütüphanesini kullanarak Python sunucusuna bağlanır. İstemci, sunucunun "Temperature" değişkeninin mevcut değerini okur ve ardından yeni bir değer yazarak bu değeri 
günceller.


2. Gereksinimler

Bu projeyi çalıştırmak için aşağıdaki yazılımlara ihtiyacınız vardır:

Python 3.6 veya üstü

.NET SDK 6.0 veya üstü (C# projesini çalıştırmak için)


3. Kurulum

Python Sunucusu:

Python ortamınızda gerekli kütüphaneyi yükleyin:

pip install opcua

server.py dosyasını çalıştırın:

python server.py

Sunucu başlatıldıktan sonra terminalde endpoint (uç nokta) adresini göreceksiniz.

C# İstemcisi:

Projenin bulunduğu dizinde (veya yeni bir konsol uygulaması oluşturup koda yapıştırarak) OPC UA istemci kütüphanesini yükleyin:

dotnet add package Opc.Ua.Client:

Bu komut, projenize Opc.Ua.Client paketini ekler.

Program.cs dosyasını çalıştırın:

dotnet run

İstemci, çalışan Python sunucusuna otomatik olarak bağlanmaya çalışacaktır.


4. Kullanım

Öncelikle Python Sunucusunu (server.py) çalıştırın.

Ardından C# İstemcisini (Program.cs) çalıştırın.

İstemci başarılı bir şekilde bağlandığında, sunucudaki sıcaklık değerini okuyacak, yeni bir değer yazacak ve güncel değeri tekrar okuyacaktır. İşlem adımları terminalde yazdırılacaktır.

Sunucuyu durdurmak için terminalde Ctrl + C tuş kombinasyonunu kullanın.


5. Ek Notlar

Bu proje, güvenlik ayarları devre dışı bırakılmış (SecurityMode.None) basit bir bağlantı örneğidir. Üretim ortamlarında güvenlik sertifikalarının kullanılması şiddetle tavsiye edilir.

Her iki kod da, sunucu ve istemcinin aynı makinede çalıştığı varsayımına dayanmaktadır. Farklı makinelerde çalıştırmak için localhost veya 127.0.0.1 yerine sunucunun IP adresini kullanmanız gerekecektir.
