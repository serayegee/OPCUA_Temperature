using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Namespace index değişkeni (Python server'dan)
        ushort idx = 2;

        // Client uygulama ayarları
        var config = new ApplicationConfiguration()
        {
            ApplicationName = "OPCUaClient",
            ApplicationType = ApplicationType.Client,
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier(),
                AutoAcceptUntrustedCertificates = true,
                TrustedPeerCertificates = new CertificateTrustList { StorePath = "pki/peer" },
                TrustedIssuerCertificates = new CertificateTrustList { StorePath = "pki/issuer" },
                RejectedCertificateStore = new CertificateTrustList { StorePath = "pki/rejected" }
            },
            TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 }
        };

        await config.Validate(ApplicationType.Client);
        config.CertificateValidator.CertificateValidation += (s, e) => { e.Accept = true; };

        try
        {
            string endpointURL = "opc.tcp://127.0.0.1:4840";

            // Endpoint discovery yap
            Console.WriteLine($"Server'dan endpoint'ler keşfediliyor: {endpointURL}");
            using var discoveryClient = DiscoveryClient.Create(new Uri(endpointURL));
            var endpoints = await discoveryClient.GetEndpointsAsync(null);

            Console.WriteLine($"Bulunan endpoint sayısı: {endpoints.Count}");

            // Bulunan endpoint'leri listele
            for (int i = 0; i < endpoints.Count; i++)
            {
                Console.WriteLine($"Endpoint {i}: {endpoints[i].EndpointUrl} - Security: {endpoints[i].SecurityMode}");
            }

            // SecurityMode.None olan endpoint'i seç
            var selectedEndpoint = endpoints.FirstOrDefault(e => e.SecurityMode == MessageSecurityMode.None);
            if (selectedEndpoint == null)
            {
                Console.WriteLine("Güvenli olmayan endpoint bulunamadı, ilk endpoint'i kullanıyoruz");
                selectedEndpoint = endpoints[0];
            }

            Console.WriteLine($"Seçilen endpoint: {selectedEndpoint.EndpointUrl}");
            Console.WriteLine($"Güvenlik Modu: {selectedEndpoint.SecurityMode}");
            Console.WriteLine($"Güvenlik Politikası: {selectedEndpoint.SecurityPolicyUri}");

            var endpointConfiguration = EndpointConfiguration.Create(config);
            var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);

            // Anonymous bağlanmak için null kullan
            UserIdentity identity = null;

            // Session oluşturma
            using var session = await Session.Create(
                config,
                endpoint,
                false,
                "OPC UA Client",
                60000,
                identity,
                null
            );

            Console.WriteLine("Server'a bağlanıldı.");
            Console.WriteLine($"Aktif endpoint: {selectedEndpoint.EndpointUrl}");

            await Task.Delay(2000);

            // Node okuma
            //var nodeId = NodeId.Parse($"ns={idx};s=MyObject.Temperature");
            var nodeId = new NodeId(2,idx);

            try
            {
                // Doğru NodeId ile okuma yap
                DataValue val = session.ReadValue(nodeId);
                Console.WriteLine("Başlangıç Sıcaklığı: " + val.Value);

                // Değer yazma işlemi
                WriteValue nodeToWrite = new WriteValue
                {
                    NodeId = nodeId,
                    AttributeId = Attributes.Value,
                    Value = new DataValue
                    {
                        Value = 30.0,
                        StatusCode = StatusCodes.Good,
                        ServerTimestamp = DateTime.UtcNow,
                        SourceTimestamp = DateTime.UtcNow
                    }
                };

                WriteValueCollection nodesToWrite = new WriteValueCollection { nodeToWrite };
                StatusCodeCollection results;
                DiagnosticInfoCollection diagnosticInfos;

                session.Write(null, nodesToWrite, out results, out diagnosticInfos);

                if (StatusCode.IsGood(results[0]))
                {
                    Console.WriteLine("Sıcaklık değeri 30 olarak yazıldı.");
                }
                else
                {
                    Console.WriteLine("Yazma başarısız: " + results[0]);
                }

                // Güncel değeri oku
                val = session.ReadValue(nodeId);
                Console.WriteLine("Güncellenen Sıcaklık: " + val.Value);
            }
            catch (Exception ex)
            {
                // Hata durumunda sadece hatayı yazdırıp çıkın.
                Console.WriteLine($"Hata: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }

        Console.WriteLine("Çıkmak için bir tuşa bas...");
        Console.ReadKey();
    }
}