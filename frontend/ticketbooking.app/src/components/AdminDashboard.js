import React, { useState, useEffect } from "react";
import signalRService from "../services/signalr.service";

const AdminDashboard = () => {
  const [tickets, setTickets] = useState([]);

  useEffect(() => {
    const connection = signalRService.getConnection();

    const handleTicketUpdate = (ticketId, user, status, companyId) => {
      setTickets(prevTickets => [...prevTickets, { ticketId, user, status, companyId }]);
    };

    connection.on("ReceiveTicketUpdate", handleTicketUpdate);

    return () => {
      connection.off("ReceiveTicketUpdate", handleTicketUpdate);
    };
  }, []);

  return (
    <div>
      <h2>Admin Dashboard - Ticket Updates</h2>
      <ul>
        {tickets.map((ticket, index) => (
          <li key={index}>
            {ticket.user} booked Ticket ID: {ticket.ticketId} - Status: {ticket.status} (Company ID: {ticket.companyId})
          </li>
        ))}
      </ul>
    </div>
  );
};

export default AdminDashboard;
