"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { registerUser } from "../../utils/requests";
import styles from "../auth.module.css";
import Link from "next/link";

export default function RegisterPage() {
  const [username, setUsername] = useState("");
  const [nickname, setNickname] = useState("");
  const [password, setPassword] = useState("");
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(false);
  const router = useRouter();

  function validatePassword() {
    if (password.length < 8) {
      setMessage("Password must be at least 8 characters long.");
      return false;
    }
    if (password.toLowerCase().includes(username.toLowerCase())) {
      setMessage("Password cannot contain your username.");
      return false;
    }
    return true;
  }

  async function handleRegister(e) {
    e.preventDefault();
    setMessage("");

    // Run validation before making API call
    if (!validatePassword()) return;

    setLoading(true);

    try {
      await registerUser({
        username,
        nickname,
        password, // backend expects this as passwordHash
        role: "User",
      });

      setMessage("Registration successful! Redirecting...");
      setTimeout(() => router.push("/login"), 1500);
    } catch (err) {
      console.error(err);
      setMessage("Registration failed. Please check your inputs.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className={styles.authWrapper}>
      <h1 className={styles.title}>Register </h1>
      <form className={styles.authForm} onSubmit={handleRegister}>
        <input
          type="text"
          placeholder="Username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          className={styles.inputField}
          required
        />
        <input
          type="text"
          placeholder="Nickname"
          value={nickname}
          onChange={(e) => setNickname(e.target.value)}
          className={styles.inputField}
          required
        />
        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          className={styles.inputField}
          required
        />
        <button type="submit" className={styles.authButton} disabled={loading}>
          {loading ? "Registering..." : "Register"}
        </button>
      </form>

      {message && <p className={styles.errorMessage}>{message}</p>}

      <p className={styles.authText}>
        Already have an account?{" "}
        <Link href="/login" className={styles.link}>
          Login
        </Link>
      </p>
    </div>
  );
}
