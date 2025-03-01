import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

const companyId = "3";

class SignalRService {
  constructor() {
    if (!SignalRService.instance) {
      this.connection = new HubConnectionBuilder()
        .withUrl("https://localhost:7093/ticketHub")
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
