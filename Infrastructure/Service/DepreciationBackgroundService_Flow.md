# Tài liệu luồng chạy: DepreciationBackgroundService

Tài liệu này giải thích chi tiết cách thức hoạt động của hệ thống khấu hao tài sản cố định tự động trong dự án NgocDaiAPI.

## 1. Tổng quan
`DepreciationBackgroundService` là một **BackgroundService** (dịch vụ chạy ngầm) được tích hợp vào ASP.NET Core module. Nhiệm vụ chính của nó là tự động tính toán giá trị hao mòn và cập nhật giá trị còn lại của tài sản cố định hàng ngày.

---

## 2. Chu kỳ hoạt động
- **Thời gian chạy**: Kích hoạt ngay khi ứng dụng khởi động.
- **Tần suất**: Lặp lại mỗi **24 giờ** một lần (`TimeSpan.FromDays(1)`).
- **Log chuẩn**: Các log quan trọng được đính kèm emoji `🧑‍💻` để tự động gửi thông báo về Discord Webhook theo cấu hình hệ thống.

---

## 3. Luồng xử lý chi tiết (Workflow)

### Bước 1: Khởi tạo và Quét dữ liệu
- Dịch vụ khởi tạo một `Scope` mới để truy cập vào `IUnitOfWork`.
- Lấy danh sách tất cả các tài sản cố định (`FixedAssets`) chưa bị xóa (`!IsDeleted`).

### Bước 2: Xác định Vòng đời (Useful Life)
- Hệ thống kiểm tra cột `DepreciationRate` của tài sản:
    - Nếu đã có Tỷ lệ (Rate > 0): Vòng đời được tính ngược lại từ Tỷ lệ (`100 / Rate`).
    - Nếu chưa có Tỷ lệ (Rate = 0): Mặc định vòng đời là **10 năm**.

### Bước 3: Logic "Truy thu" lịch sử (Historical Catch-up)
Đây là phần quan trọng nhất, giúp hệ thống tự động điền dữ liệu nếu tài sản được nhập vào từ lâu nhưng chưa có bản ghi khấu hao:
- Hệ thống duyệt từ tháng chứa `UsageTime` (Ngày bắt đầu dùng) cho đến tháng hiện tại.
- Với mỗi tháng trong khoảng thời gian này:
    - Kiểm tra xem bảng `DepreciationDetails` đã có bản ghi cho tháng đó chưa.
    - Nếu **chưa có**: Tạo mới một bản ghi chi tiết khấu hao (`Amounts`, `Note`, `CreatedDate`).
    - Nếu **đã có**: Bỏ qua và chuyển sang tháng tiếp theo.

### Bước 4: Tính toán số liệu theo công thức chuẩn
Hệ thống áp dụng bộ công thức kế toán nghiêm ngặt:
1.  **Tỷ lệ hao mòn năm** = `100 / Vòng đời (năm)`
2.  **Mức khấu hao tháng** = `(Nguyên giá * Tỷ lệ năm) / 100 / 12`
3.  **Số năm đã dùng** = `Số ngày thực tế tính từ ngày bắt đầu dùng / 365`
4.  **Hao mòn lũy kế** = `(Số ngày thực tế tính từ ngày bắt đầu dùng / 30) * Mức khấu hao tháng`
5.  **Giá trị còn lại** = `Nguyên giá - Hao mòn lũy kế`

### Bước 5: Cập nhật Master Data (FixedAssets)
Sau khi tính toán xong, các trường sau trên bảng `FixedAssets` sẽ được cập nhật lại giá trị mới nhất:
- `UsageYear`: Cập nhật lại số năm đã dùng thực tế (ví dụ: 1.25 năm).
- `AccumulatedDepreciation`: Tổng giá trị đã hao mòn.
- `RemainingValue`: Giá trị còn lại của tài sản.
- `DepreciationValue`: Mức khấu hao của một tháng.

### Bước 6: Lưu dữ liệu (Commit)
- Toàn bộ các bản ghi `DepreciationDetails` mới và các cập nhật trên `FixedAssets` được lưu vào Database trong một Transaction duy nhất thông qua `unitOfWork.CommitAsync()`.

---

## 4. Các điểm lưu ý kỹ thuật
- **Làm tròn số**: Toàn bộ số tiền và tỉ lệ đều được làm tròn 2 chữ số thập phân (`Math.Round(..., 2)`) để đảm bảo tính thẩm mỹ và chính xác khi hiển thị.
- **An toàn dữ liệu**: Hệ thống sử dụng `Dapper` và `Generic Repository` để tối ưu hiệu năng khi xử lý danh sách tài sản lớn.
- **Xử lý ngoại lệ**: Mọi lỗi xảy ra trong quá trình tính toán đều được bắt lại (`catch`), log lỗi chi tiết và gửi về Discord, đảm bảo dịch vụ không bị chết đột ngột.

---

> [!TIP]
> **Mách nhỏ**: Bạn có thể theo dõi tiến trình này thông qua Debug Console hoặc Discord Webhook của hệ thống với từ khóa `DepreciationBackgroundService`.
