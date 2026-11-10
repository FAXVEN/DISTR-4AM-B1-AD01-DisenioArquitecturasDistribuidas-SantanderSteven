# AD01 · Comercio multibodega

Tres proyectos ASP.NET Core Web API para .NET 10. Cada uno tiene su propia solución y usa su propia base de datos. Las API se pueden ejecutar por separado.

| Solución | Base de datos | Swagger | Operaciones |
| --- | --- | --- | --- |
| [Catalogo.API](Catalogo.API/Catalogo.API.slnx) | CatalogoDB | https://localhost:7029/swagger/index.html | CRUD de categorías, marcas y productos |
| [Inventario.API](Inventario.API/Inventario.API.slnx) | InventarioDB | https://localhost:7036/swagger/index.html | CRUD de bodegas, consulta de existencias, registro y consulta de movimientos, registro, consulta y cancelación de reservas |
| [Pedidos.API](Pedidos.API/Pedidos.API.slnx) | PedidosDB | https://localhost:7048/swagger/index.html | CRUD de clientes, registro y consulta de pedidos con detalle |

## Ejecutar y probar

1. Usa Visual Studio con soporte para .NET 10. En SSMS conéctate a `(localdb)\MSSQLLocalDB` con autenticación de Windows y comprueba que estén las tres bases con sus tablas. En otro equipo, prepara primero los esquemas. Para exportarlos desde el equipo original, en SSMS usa **Tareas → Generar scripts** en cada base y selecciona **Solo esquema**. Eso permite compartir tablas y relaciones sin datos de prueba ni archivos de base.
2. Abre una solución, selecciona su perfil **https** y pulsa **F5**.
3. Importa [AD01.postman_collection.json](AD01.postman_collection.json) en Postman. En el Runner ejecuta la carpeta de la API encendida. Repite el procedimiento con las otras soluciones. Ajusta las variables de URL si Visual Studio asigna otro puerto.
4. Guarda la evidencia del Runner. Ejecuta cada carpeta en orden, porque varias solicitudes reutilizan los identificadores devueltos por las anteriores.

**Resultado verificado con Newman:** 68 solicitudes y 91 comprobaciones, todas correctas. La ejecución automatizada usó puertos HTTP temporales. La colección importable usa los puertos HTTPS de Visual Studio. Las pruebas de Inventario y Pedidos conservan los registros de historial creados durante la ejecución. [Guía del Runner de Postman](https://learning.postman.com/docs/tests-and-scripts/running-collections/intro-to-collection-runs/)

## Llaves físicas y referencias lógicas

| Base | Llaves foráneas físicas verificadas en SQL Server |
| --- | --- |
| CatalogoDB | Producto.IdCategoria → Categoria.IdCategoria y Producto.IdMarca → Marca.IdMarca |
| InventarioDB | Existencia.IdBodega, MovimientoInventario.IdBodega y ReservaStock.IdBodega → Bodega.IdBodega |
| PedidosDB | Pedido.IdCliente → Cliente.IdCliente y DetallePedido.IdPedido → Pedido.IdPedido |

Las relaciones físicas bloquean el borrado de filas referenciadas, salvo DetallePedido → Pedido, que tiene borrado en cascada. IdProducto en Inventario y en DetallePedido es una referencia lógica. Las pruebas usan el ID de ejemplo 1001 para que esas API puedan funcionar de forma independiente. [Claves foráneas en EF Core](https://learn.microsoft.com/en-us/ef/core/modeling/relationships/foreign-and-principal-keys)

La prueba de Inventario registra una entrada de 10, reserva 4, comprueba el rechazo de una salida de 7, cancela la reserva y registra la salida. El stock y el disponible terminan en 3. La prueba de Pedidos guarda dos unidades a 25,50, consulta el detalle y verifica un total de 51,00.

## Entregar en GitHub

1. Completa tu nombre y revisa las fuentes en el [registro de uso de IA](Registro-IA-AD01.pdf).
2. Crea un repositorio vacío llamado `DISTR-NIVELPARALELO-B1-AD01-DisenioArquitecturasDistribuidas-ApellidoNombre`. Sustituye NIVELPARALELO y ApellidoNombre por tus datos. En el ejemplo del docente, 4AM significa Cuarto A Matutino.
3. Abre PowerShell **dentro de Deber hecho por mi** y ejecuta:

   ```powershell
   git init -b main
   git add .
   git status --short
   ```

   Revisa que figuren los tres proyectos, el README, la colección y el PDF. El archivo .gitignore excluye bin, obj y .vs. Revisa también que tus archivos de configuración estén libres de credenciales antes de publicar.
4. Publica en el repositorio que creaste, reemplazando URL_DE_TU_REPOSITORIO por su dirección:

   ```powershell
   git commit -m "Entrega AD01 comercio multibodega"
   git remote add origin URL_DE_TU_REPOSITORIO
   git push -u origin main
   ```

5. Abre el repositorio en GitHub, comprueba que se vean las tres soluciones y los archivos de entrega, y envía el enlace por el medio indicado por tu docente. Adjunta el PDF por separado si te lo pide la plataforma.
