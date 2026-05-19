# ASPNET-DX22TT8-TranHoaiThuc-BanGiay

---

> 💻 Chào mừng bạn đến với tài liệu thiết lập môi trường lập trình!  
> Làm theo các mục dưới đây theo thứ tự để hoàn tất cài đặt.

---

## Mục Lục

- [Phần A — Cài Đặt Visual Studio](#phần-a--cài-đặt-visual-studio)
- [Phần B — Cài Đặt .NET SDK](#phần-b--cài-đặt-net-sdk)
- [Phần C — Tạo Dự Án Đầu Tiên](#phần-c--tạo-dự-án-đầu-tiên)
- [Phần D — Cài Đặt SQL Server Management Studio](#phần-d--cài-đặt-sql-server-management-studio)
- [Phần E — Kết Nối Database](#phần-e--kết-nối-database)

---

## Phần A — Cài Đặt Visual Studio

Hướng dẫn này sẽ giúp bạn thiết lập **Visual Studio** để đảm bảo bạn có mọi thứ cần thiết cho các dự án lập trình của mình! 💡

### A.1 · Tải xuống trình cài đặt

Truy cập liên kết bên dưới để tải Visual Studio về máy:

📥 **[https://visualstudio.microsoft.com/downloads/](https://visualstudio.microsoft.com/downloads/?cid=learn-onpage-download-install-visual-studio-page-cta/)**

Tại trang tải xuống, chọn phiên bản **`Community`** → nhấn **`Free Download`** để tải IDE về máy.

Sau khi tải xong, mở file cài đặt để bắt đầu.

---

### A.2 · Cấu hình và cài đặt

Nhấp đúp vào file đã tải để khởi chạy trình cài đặt, sau đó lần lượt cấu hình theo các tab:

| Tab | Việc cần làm |
|---|---|
| `Workloads` | Chọn **`ASP.NET and web development`** |
| `Individual components` | Tùy chỉnh các phiên bản framework nếu cần |
| `Language packs` | Chọn ngôn ngữ hiển thị |
| `Installation Locations` | Đổi đường dẫn cài đặt nếu muốn |

Sau khi cấu hình xong, nhấn **`Install`** và chờ quá trình cài đặt hoàn tất.

🎉 Khi hoàn tất, mở **Visual Studio** từ Menu Start hoặc màn hình chính.

---

## Phần B — Cài Đặt .NET SDK

### B.1 · Tải xuống .NET SDK

Truy cập trang chính thức để tải phiên bản mới nhất:

📥 **[https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)**

### B.2 · Cài đặt

Làm theo hướng dẫn cài đặt dành cho hệ điều hành của bạn — Windows, macOS hoặc Linux.

### B.3 · Xác minh cài đặt thành công

Mở **Terminal** hoặc **Command Prompt** và chạy lệnh sau:

```bash
dotnet --version
```

Nếu màn hình hiển thị số phiên bản (ví dụ: `8.0.100`), bạn đã cài đặt thành công.

---

## Phần C — Tạo Dự Án Đầu Tiên

### C.1 · Tạo project mới trong Visual Studio

Mở Visual Studio, sau đó điều hướng theo đường dẫn:

```
File  →  Create a new project  →  ASP.NET Core Web App (Model - View - Controller)
```

### C.2 · Di chuyển vào thư mục dự án

Sau khi tạo dự án, mở thư mục chứa project của bạn để bắt đầu làm việc.

### C.3 · Chạy dự án

Mở file solution trong thư mục dự án:

```
your_project.sln
```

Sau đó nhấn tổ hợp phím để chạy:

```
Ctrl + F5
```

🎈 **Chúc mừng! Ứng dụng C# của bạn đã chạy thành công!**

---

## Phần D — Cài Đặt SQL Server Management Studio

### D.1 · Tải xuống SSMS

📥 **[Tải SSMS 20.2](https://aka.ms/ssmsfullsetup)**

> **Lưu ý quan trọng trước khi cài:**
> - SSMS 20.2 là phiên bản GA (Generally Available) mới nhất.
> - Nếu bạn đang có **phiên bản Preview của SSMS 20**, hãy **gỡ cài đặt trước** rồi mới cài 20.2.
> - SSMS 20.2 **không** nâng cấp hay thay thế SSMS 19.x và các phiên bản cũ hơn — chúng có thể tồn tại song song trên cùng một máy.
> - Nếu có nhiều phiên bản SSMS cùng lúc, hãy chắc chắn bạn mở đúng phiên bản cần dùng. Phiên bản mới nhất được gắn nhãn **`Microsoft SQL Server Management Studio v20.2`**.

### D.2 · Cài đặt

Chạy file vừa tải về:

```
SSMS-Setup-ENU.exe
```

Làm theo hướng dẫn trên màn hình. Sau khi hoàn tất, bạn đã cài đặt thành công SQL Server Management Studio.

---

## Phần E — Kết Nối Database

### E.1 · Kết nối SQL Server trong Visual Studio

Thực hiện lần lượt các bước sau trong Visual Studio:

```
Bước 1  →  Chọn Tools  >  Connect to Database...
Bước 2  →  Điền Server name (server đã tạo trong SSMS)
Bước 3  →  Nhấn Refresh
Bước 4  →  Chọn Database tại ô "Select or enter a database name"
Bước 5  →  Nhấn Test Connection  >  OK
```

### E.2 · Lấy Connection String

Sau khi kết nối thành công, lấy chuỗi kết nối theo đường dẫn:

```
Server Explorer  →  Data Connections  →  Chuột phải vào Database  →  Properties
```

Sao chép giá trị **`Connection String`** hiển thị trong cửa sổ Properties.

### E.3 · Cấu hình vào dự án

Dán chuỗi kết nối vừa sao chép vào file:

```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "your_connection_string_here"
  }
}
```

✅ Hoàn tất! Môi trường phát triển của bạn đã sẵn sàng.


