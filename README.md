<div align="center">

# 📡 Thực hành NT106 — Lập trình mạng căn bản

![C#](https://img.shields.io/badge/C%23-.NET%20Framework-239120?style=for-the-badge&logo=csharp&logoColor=white)
![WinForms](https://img.shields.io/badge/WinForms-Desktop%20App-0078D4?style=for-the-badge&logo=windows&logoColor=white)
![TCP](https://img.shields.io/badge/TCP-Socket%20Programming-red?style=for-the-badge)

*Tổng hợp các bài Lab môn NT106 (Lập trình mạng căn bản) — University of Information Technology, VNU-HCM.*

</div>

---

## 📖 Giới thiệu

Repo tổng hợp bài thực hành qua các buổi Lab của môn **NT106**, từ làm quen WinForms cơ bản (Lab02, Lab04) đến lập trình mạng Socket TCP thực tế (Lab06 — ứng dụng Whiteboard cộng tác nhiều người dùng thời gian thực).

---

## 📂 Cấu trúc thư mục

| Thư mục | Nội dung | Công nghệ |
| :--- | :--- | :--- |
| [`Lab02/`](Lab02) | Bài tập WinForms cơ bản: Menu, quản lý sinh viên (`Student.cs`) | C# WinForms |
| [`Lab03/`](Lab03) | Bài tập thực hành thêm | C# |
| [`Lab04/`](Lab04) | Bài tập WinForms nâng cao (Bai01-Bai04), Menu điều hướng | C# WinForms |
| [`Lab05/`](Lab05) | Bài tập thực hành thêm | C# |
| [`Lab06/`](Lab06) | **Ứng dụng Whiteboard cộng tác qua mạng (Client-Server, TCP Socket)** | C# WinForms + Console App |

---

## 🖊️ Lab06 — Collaborative Whiteboard (trọng tâm)

Ứng dụng bảng vẽ trắng (Whiteboard) cho phép **nhiều Client kết nối tới 1 Server** qua giao thức **TCP Socket**, vẽ và đồng bộ hình vẽ/hình dạng/hình ảnh theo thời gian thực với tất cả người dùng khác đang kết nối.

### Kiến trúc

```
   Client A (WinForms) ─┐
   Client B (WinForms) ─┼──▶  Server (Console App, TCP Socket, port 9000)
   Client C (WinForms) ─┘
```

- **`Lab06/Server/`** — Console App, lắng nghe kết nối TCP tại **port 9000**, quản lý danh sách client, broadcast lại thao tác vẽ/xoá/ảnh cho tất cả client khác.
- **`Lab06/Client/`** — WinForms App, giao diện vẽ (nét vẽ, hình dạng, chèn ảnh, kéo-thả), gửi/nhận dữ liệu qua Socket tới Server.
- **`Lab06/Pictures/`** — Thư mục app tự tạo để lưu ảnh khi bấm "Save Image" lúc chạy demo (không commit lên Git, xem `.gitignore`).

### Tính năng chính

- ✏️ Vẽ tự do (freehand), vẽ hình dạng (đường thẳng, hình chữ nhật, hình tròn...)
- 🖼️ Chèn ảnh, kéo-thả nhiều ảnh cùng lúc trên bảng vẽ
- 💾 Lưu ảnh bảng vẽ (composite toàn bộ nét vẽ + hình dạng + ảnh chèn vào 1 file ảnh xuất ra)
- 🔄 Đồng bộ real-time giữa nhiều Client cùng kết nối vào 1 Server
- 🛡️ Xử lý exception toàn diện phía Client/Server (mất kết nối, lỗi socket, dữ liệu không hợp lệ...)

### Cách chạy thử

1. Mở `Lab06/Server/Server.sln` bằng Visual Studio → build & chạy trước (Console App sẽ in `"Server started on port 9000"`).
2. Mở `Lab06/Client/Client.sln` → build & chạy (có thể chạy nhiều instance `.exe` để giả lập nhiều người dùng).
3. Trên mỗi Client, nhập đúng địa chỉ IP của máy chạy Server (dùng `localhost` nếu test trên cùng máy) để kết nối vào port `9000`.
4. Vẽ trên 1 Client → các Client khác đang kết nối sẽ thấy đồng bộ ngay lập tức.

---

## ⚙️ Yêu cầu chung

- **Windows** + **Visual Studio** (2019/2022) — các project đều là WinForms/.NET Framework, không chạy được trên Linux/macOS.
- Mỗi thư mục `LabXX/` có file `.sln` riêng, mở solution tương ứng để build/chạy độc lập từng bài.

---

<div align="center">

*Đồ án thực hành — NT106, UIT (VNU-HCM)*

</div>
