"use client";

import { createContext, useState, useEffect } from "react";

export const UserContext = createContext({
  user: null,
  initialized: false, // ✅ track if context is ready
  login: () => {},
  logout: () => {},
});

export function UserProvider({ children }) {
  const [user, setUser] = useState(null);
  const [initialized, setInitialized] = useState(false);

  useEffect(() => {
    const storedUser = localStorage.getItem("user");
    if (storedUser) {
      setUser(JSON.parse(storedUser));
    }
    setInitialized(true); // ✅ context finished loading
  }, []);

  const login = (userData) => {
    setUser(userData);
    localStorage.setItem("user", JSON.stringify(userData));
  };

  const logout = () => {
    setUser(null);
    localStorage.removeItem("user");
  };

  return (
    <UserContext.Provider value={{ user, initialized, login, logout }}>
      {children}
    </UserContext.Provider>
  );
}
