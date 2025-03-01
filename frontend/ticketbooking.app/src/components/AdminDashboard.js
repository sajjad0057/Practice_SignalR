import React, { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";

const AdminDashboard = () => {
  const [tickets, setTickets] = useState([]);

  useEffect(() => {
      const connection = new signalR.HubConnectionBuilder()
          .withUrl("https://localhost:7093/ticketHub")
          .withAutomaticReconnect()
          .build();

      connection.start()
          .then(() => console.log("Connected to SignalR"))
          .catch(err => console.error("Connection failed:", err));

      connection.on("ReceiveTicketUpdate", (ticketId, user, status) => {
          setTickets(prevTickets => [...prevTickets, { ticketId, user, status }]);
      });

      return () => {
          connection.stop();
      };
  }, []);

  return (
      <div>
          <h2>Admin Dashboard - Ticket Updates</h2>
          <ul>
              {tickets.map((ticket, index) => (
                  <li key={index}>{ticket.user} booked Ticket ID: {ticket.ticketId} - Status: {ticket.status}</li>
              ))}
          </ul>
      </div>
  );
};

export default AdminDashboard