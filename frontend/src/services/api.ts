import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:5185/api",
  headers: {
    "Content-Type": "application/json",
  },
});

export const login = async (data: {
  username: string;
  password: string;
}) => {
  const response = await api.post("/Auth/login", data);
  return response.data;
};

export default api;