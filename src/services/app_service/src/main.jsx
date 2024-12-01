import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import "./styles/index.scss";
import App from './App.jsx'
import nodemon from 'nodemon';



createRoot(document.getElementById('root')).render(
  <StrictMode>
    <App />
    
  </StrictMode>,
)
