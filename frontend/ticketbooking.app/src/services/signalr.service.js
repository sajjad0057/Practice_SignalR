import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

const companyId = "3";
const token = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNyc2Etc2hhMjU2IiwidHlwIjoiSldUIn0.eyJzZXNzaW9uIjoiMTc0ZGE3MzItZjc1Yy0xMWVmLTlkYjUtMzA4NWE5ZWUyNWQ0LWMwOWNhZjZkNGNhNTNjMWJlNmY3ZTlmOWJkYWIwNzEzIiwibmJmIjoxNzQwOTE2MDM3LCJleHAiOjE3NDM1MDgwMzcsImlzcyI6InF1aW5ib29rLmNvbSIsImF1ZCI6InF1aW5ib29rLmNvbSJ9.DcbDmsArrNm1vEfWNz5ArJJFnqI-aBubK1jUxjvv6t9TS0EZRL6PvtzIt7hzu1SHQWrierBHcIXuC4aW62oy32FgFEwKAUhECGseDca5ItUfaeCWG8JtNuL9ThMM-KgqKNVsFm1J0CMHSLnFNpMGri9UEanhY1mhelA3yY9uvYfStqXCM_KcYJv1QCJnbQaMyRkNSOyTyoubUMdbF_hbN3CVbcpG68Cte0vd5XW7KskFxmnClr7qOWwMNuJ960SZVKiGt9PoG3LF4r-mI29uw0R__Q_vx36PyqBuU4KTeqqVVr8lIMDZh3LE4kIuCHa-evKUtIpNIAiICVvHFfYN6w";

class SignalRService {
  
  constructor() {
    if (!SignalRService.instance) {
      this.connection = new HubConnectionBuilder()
        .withUrl("http://localhost:5000/ticketHub", {
          accessTokenFactory: () => token // Attach token to the request
        })
        .configureLogging(LogLevel.Information)
        .withAutomaticReconnect()
        .build();

      this.startConnection();
      SignalRService.instance = this;
    }
    return SignalRService.instance;
  }

  async startConnection() {
    try {
      await this.connection.start();
      console.log("Connected to SignalR with Company Id:", companyId);
      await this.connection.invoke("JoinCompanyGroup", companyId);
    } catch (err) {
      console.error("Connection failed:", err);
      setTimeout(() => this.startConnection(), 5000); // Retry connection after 5 seconds
    }
  }

  getConnection() {
    return this.connection;
  }
}

const signalRService = new SignalRService();
export default signalRService;
