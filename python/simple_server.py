from opcua import Server
import time

def main():
    # Server oluşturma
    server = Server()
    
    # Endpoint ayarlama
    server.set_endpoint("opc.tcp://localhost:4840")
    
    # Server adı
    server.set_server_name("Simple OPC UA Server")
    
    # Namespace kaydetme
    uri = "http://examples.freeopcua.github.io"
    idx = server.register_namespace(uri)
    
    # Objects node'unu al
    objects = server.get_objects_node()
    
    # Yeni bir object ekle
    myobject = objects.add_object(idx, "MyObject")
    
    # Variable ekle
    temperature = myobject.add_variable(idx, "Temperature", 25.0)
    temperature.set_writable()  # Client'ın yazmasına izin ver
    
    # Server'ı başlat
    server.start()
    print("OPC UA Server başlatıldı!")
    print("Endpoint: opc.tcp://localhost:4840")
    print("Namespace Index:", idx)

    print("Gerçek Temperature Node ID:", temperature.nodeid)
    #print("Temperature Node ID: ns={};s=Temperature".format(idx))
    
    try:
        count = 0
        while True:
            # Her 2 saniyede bir temperature değerini artır
            current_val = temperature.get_value()
            new_val = 20.0 + (count % 50)  # 20-70 arasında değişen değer
            temperature.set_value(new_val)
            print(f"Temperature güncellendi: {new_val}°C")
            count += 1
            time.sleep(2)
            
    except KeyboardInterrupt:
        print("\nServer durduruluyor...")
    finally:
        server.stop()
        print("Server durduruldu!")

if __name__ == "__main__":
    main()