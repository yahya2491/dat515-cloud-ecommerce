// src/utils/session.js

// Save logged-in user to localStorage
export function setUser(user) {
  localStorage.setItem("user", JSON.stringify(user));
}

// Get the currently logged-in user
export function getUser() {
  const user = localStorage.getItem("user");
  return user ? JSON.parse(user) : null;
}

// Clear user session (logout)
export function clearUser() {
  localStorage.removeItem("user");
}

// Check if user is logged in
export function isLoggedIn() {
  return !!getUser();
}
