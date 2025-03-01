import 'bootstrap/dist/css/bootstrap.min.css';
import { Container } from "react-bootstrap";
import AdminDashboard from './components/AdminDashboard';

function App() {
  return (
    <div className="App">
      <Container>
        <h2>Book your Ticket here : </h2>
      </Container>
      <hr/>
      <AdminDashboard/>
    </div>
  );
}

export default App;
