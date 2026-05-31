// src/utils/requests.js

// Helper function to get API URL with runtime config
function getApiUrl() {
  // For Next.js 13+ with App Directory, we use environment variables directly
  // The runtime configuration will be available through process.env on both server and client
  return process.env.NEXT_PUBLIC_API_URL || "http://localhost:8080";
}

const ADMIN_KEY = "123-admin-key";

// ====================  USER REQUESTS ====================

export async function registerUser({ username, nickname, password }) {
  const API_URL = getApiUrl();
  const res = await fetch(`${API_URL}/api/User/register`, {
    method: "POST",
    headers: {
      "username": username,
      "nickname": nickname,
      "password": password
    },
  });

  if (!res.ok) {
    throw new Error("Failed to register");
  }

  return res.json();
}

export async function loginUser(username, password) {
  const API_URL = getApiUrl();
  const res = await fetch(`${API_URL}/api/User/login`, {
    method: "POST",
    headers: {
      "username": username,
      "password": password
    }
  });

  if (!res.ok) {
    throw new Error("Invalid username or password");
  }

  return res.json();
}

export async function getUserById(id) {
  const API_URL = getApiUrl();
  try {
    const res = await fetch(`${API_URL}/api/User/${id}`);
    if (!res.ok) throw new Error("Failed to fetch user");
    const data = await res.json();
    return data;
  } catch (err) {
    console.error(err);
    throw err;
  }
}

// ====================  ADMIN REQUESTS ====================

// Get all users
export async function getAllUsers() {
  const API_URL = getApiUrl();
  const res = await fetch(`${API_URL}/api/Admin/users`, {
    headers: { "X-Admin-Key": ADMIN_KEY },
  });
  if (!res.ok) throw new Error("Failed to fetch users");
  return res.json();
}

// Get user by ID
export async function getUserByIdAdmin(id) {
  const API_URL = getApiUrl();
  const res = await fetch(`${API_URL}/api/Admin/users/${id}`, {
    headers: { "X-Admin-Key": ADMIN_KEY },
  });
  if (!res.ok) throw new Error("Failed to fetch user");
  return res.json();
}

// Update user
export async function updateUser(id, data) {
  const API_URL = getApiUrl();
  const res = await fetch(`${API_URL}/api/Admin/users/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      "X-Admin-Key": ADMIN_KEY,
    },
    body: JSON.stringify(data),
  });
  if (!res.ok) throw new Error("Failed to update user");
  return res.json();
}

// Delete user
export async function deleteUser(id) {
  const API_URL = getApiUrl();
  const res = await fetch(`${API_URL}/api/Admin/users/${id}`, {
    method: "DELETE",
    headers: { "X-Admin-Key": ADMIN_KEY },
  });
  if (!res.ok) throw new Error("Failed to delete user");
  return true;
}

// Get stats
export async function getStats() {
  const API_URL = getApiUrl();
  const res = await fetch(`${API_URL}/api/Admin/stats`, {
    headers: { "X-Admin-Key": ADMIN_KEY },
  });
  if (!res.ok) throw new Error("Failed to fetch stats");
  return res.json();
}

// Track product view
export async function trackProductView(productId) {
  const API_URL = getApiUrl();
  const res = await fetch(`${API_URL}/api/Admin/track-view/${productId}`, {
    method: "POST",
    headers: { "X-Admin-Key": ADMIN_KEY },
  });
  if (!res.ok) throw new Error("Failed to track view");
  return res.json();
}

// ==================== 📦 PRODUCT REQUESTS ====================

export async function getAllProducts() {
  const API_URL = getApiUrl();
  const res = await fetch(`${API_URL}/api/Products`);
  if (!res.ok) throw new Error("Failed to fetch products");
  return res.json();
}

export async function updateProduct(id, data, adminKey) {
  const API_URL = getApiUrl();
  const res = await fetch(`${API_URL}/api/Products/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      "X-Admin-Key": adminKey,
    },
    body: JSON.stringify(data),
  });
  if (!res.ok) throw new Error("Failed to update product");
  return res.json();
}

export async function deleteProduct(id, adminKey) {
  const API_URL = getApiUrl();
  const res = await fetch(`${API_URL}/api/Products/${id}`, {
    method: "DELETE",
    headers: {
      "X-Admin-Key": adminKey,
    },
  });
  if (!res.ok) throw new Error("Failed to delete product");
}

// ==================== 🛒 CART REQUESTS ====================

// Get a user's current cart (or create one if missing)
export async function getUserCart(userId) {
  const API_URL = getApiUrl();
  const res = await fetch(`${API_URL}/api/Cart/user/${userId}`);
  if (!res.ok) throw new Error("Failed to fetch user cart");
  return res.json();
}

// Add an item to a user's cart
export async function addItemToUserCart(userId, productId, quantity = 1) {
  const API_URL = getApiUrl();
  const res = await fetch(`${API_URL}/api/Cart/user/${userId}/add`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ productId, quantity }),
  });
  if (!res.ok) throw new Error("Failed to add item to cart");
  return res.json();
}

// Remove a product from a specific cart
export async function removeItemFromCart(cartId, productId) {
  const API_URL = getApiUrl();
  const res = await fetch(`${API_URL}/api/Cart/${cartId}/remove/${productId}`, {
    method: "DELETE",
  });
  if (!res.ok) throw new Error("Failed to remove item from cart");
  return res.json();
}

// Clear all items in a cart
export async function clearCart(cartId) {
  const API_URL = getApiUrl();
  const res = await fetch(`${API_URL}/api/Cart/${cartId}/clear`, {
    method: "DELETE",
  });

  if (!res.ok) {
    const errorData = await res.json().catch(() => ({}));
    throw new Error(errorData.message || "Failed to clear cart");
  }

  return res.json();
}


// Create a new cart for the user
export async function createCartForUser(userId) {
  const API_URL = getApiUrl();
  const res = await fetch(`${API_URL}/api/Cart/create/${userId}`, {
    method: "POST",
  });
  if (!res.ok) throw new Error("Failed to create cart");
  return res.json();
}

// Get all carts (you could filter by user in frontend)
export async function getAllCarts() {
  const API_URL = getApiUrl();
  const res = await fetch(`${API_URL}/api/Cart`);
  if (!res.ok) throw new Error("Failed to fetch carts");
  return res.json();
}

// Get a specific cart
export async function getCart(cartId) {
  const API_URL = getApiUrl();
  const res = await fetch(`${API_URL}/api/Cart/${cartId}`);
  if (!res.ok) throw new Error("Failed to fetch cart");
  return res.json();
}

// Update quantity by re-adding the product with new quantity
export async function updateCartItemQuantity(cartId, productId, quantity) {
  const API_URL = getApiUrl();
  const res = await fetch(`${API_URL}/api/Cart/${cartId}/add`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ productId, quantity }),
  });
  if (!res.ok) throw new Error("Failed to update quantity");
  return res.json();
}

// Remove an item
export async function removeCartItem(cartId, productId) {
  const API_URL = getApiUrl();
  const res = await fetch(`${API_URL}/api/Cart/${cartId}/remove/${productId}`, {
    method: "DELETE",
  });
  if (!res.ok) throw new Error("Failed to remove item");
  return res.json();
}
export async function checkoutPayment(userId, paymentRequest) {
  const API_URL = getApiUrl();
  const res = await fetch(`${API_URL}/api/Payments/checkout/${userId}`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(paymentRequest), 
  });

  let resultText;
  try {
    resultText = await res.text();
  } catch {
    resultText = "";
  }

  let jsonResult;
  try {
    jsonResult = resultText ? JSON.parse(resultText) : {};
  } catch {
    jsonResult = {};
  }

  if (!res.ok) {
    console.error("❌ Payment API Error:", res.status, jsonResult);
    throw new Error(jsonResult.message || `Payment failed (HTTP ${res.status})`);
  }

  console.log("✅ Payment API Success:", jsonResult);
  return jsonResult;
}
